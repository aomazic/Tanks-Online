using System;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class WebSocketController : MonoBehaviour
{
    private const string CHAT_TOPIC = "chat";
    private const string GAME_TOPIC = "game";
    private const string PLAYER_UPDATE_TOPIC = "game/players";
    private const string PROJECTILE_TOPIC = "game/projectiles";
    private const string BaseUrl = "ws://localhost:8080/ws/websocket";
    private WebSocket websocket;
    
    private const string SUBSCRIPTION_ID = "sub-0";
    
    public static WebSocketController Instance { get; private set; }
    
    // Callback events
    public event Action<ChatMessage> OnChatMessageReceived;
    public event Action<PlayerGameInfo> OnPlayerGameInfoReceived;
    public event Action<ProjectileEvent> OnProjectileEventReceived;
    public event Action OnConnectionOpened;
    public event Action<string> OnConnectionError;
    public event Action OnConnectionClosed;
    
    private bool isConnected;
    
    private const string STOMP_CONNECT_FRAME = "CONNECT\n" +
                                               "accept-version:1.1,1.0\n" +
                                               "heart-beat:10000,10000\n\n\0";
    
    private const string STOMP_DISCONNECT_FRAME = "DISCONNECT\n\n\0";
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public async Task ConnectToServer()
    {
        if (websocket != null)
        {
            await DisconnectFromServer();
        }

        if (!UserInfoController.IsUserAuthenticated())
        {
            Debug.LogError("User not authenticated. Cannot connect to server.");
            return;
        }

        var token = UserInfoController.GetToken();
        websocket = new WebSocket($"{BaseUrl}?token={token}");

        websocket.OnOpen += async () =>
        {
            Debug.Log("WebSocket connection opened");
            isConnected = true;

            // Send STOMP connection frame
            await SendStompConnect();

            OnConnectionOpened?.Invoke();

            var sessionId = GameSessionController.GetSessionData().sessionId;
            if (sessionId == 0)
            {
                return;
            }
    
            // Subscribe to all topics
            var sessionIdStr = sessionId.ToString();
            await SubscribeToTopic(sessionIdStr, CHAT_TOPIC);
            await SubscribeToTopic(sessionIdStr, GAME_TOPIC);
            await SubscribeToTopic(sessionIdStr, PLAYER_UPDATE_TOPIC);
            await SubscribeToTopic(sessionIdStr, PROJECTILE_TOPIC);
            
            // Join chat and game
            await JoinChat(sessionIdStr, UserInfoController.GetUsername());
            await JoinGame(sessionIdStr, UserInfoController.GetUsername());
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"WebSocket Error: {e}");
            isConnected = false;
            OnConnectionError?.Invoke(e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket connection closed");
            isConnected = false;
            OnConnectionClosed?.Invoke();
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            
            if (message.StartsWith("CONNECTED") || message.StartsWith("RECEIPT"))
            {
                Debug.Log("STOMP protocol message received");
                return;
            }

            if (message.StartsWith("MESSAGE"))
            {
                var bodyIndex = message.IndexOf("\n\n", StringComparison.Ordinal);
                if (bodyIndex == -1)
                    return;
    
                var body = message[(bodyIndex + 2)..].TrimEnd('\0');
                ProcessMessage(message, body);
                return;
            }

            // Try to process as direct JSON (fallback)
            try
            {
                ProcessMessage(message, message);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to process message: {ex.Message}");
            }
        };

        try
        {
            await websocket.Connect();
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection failed: {e.Message}");
            OnConnectionError?.Invoke(e.Message);
        }
    }
    
    private void ProcessMessage(string fullMessage, string jsonBody)
    {
        try
        {
            // Determine message type from the destination header
            if (fullMessage.Contains("/topic/chat/"))
            {
                OnChatMessageReceived?.Invoke(JsonUtility.FromJson<ChatMessage>(jsonBody));
            }
            else if (fullMessage.Contains("/topic/game/players/"))
            {
                OnPlayerGameInfoReceived?.Invoke(JsonUtility.FromJson<PlayerGameInfo>(jsonBody));
            }
            else if (fullMessage.Contains("/topic/game/projectiles/"))
            {
                OnProjectileEventReceived?.Invoke(JsonUtility.FromJson<ProjectileEvent>(jsonBody));
            }
            else
            {
                // Try to determine type based on JSON content
                if (jsonBody.Contains("\"type\":\"CHAT\"") || jsonBody.Contains("\"type\":\"JOIN\""))
                {
                    OnChatMessageReceived?.Invoke(JsonUtility.FromJson<ChatMessage>(jsonBody));
                }
                else if (jsonBody.Contains("\"playerId\"") && jsonBody.Contains("\"health\""))
                {
                    OnPlayerGameInfoReceived?.Invoke(JsonUtility.FromJson<PlayerGameInfo>(jsonBody));
                }
                else if (jsonBody.Contains("\"originX\"") && jsonBody.Contains("\"originY\""))
                {
                    OnProjectileEventReceived?.Invoke(JsonUtility.FromJson<ProjectileEvent>(jsonBody));
                }
                else
                {
                    Debug.LogWarning($"Unknown message format: {jsonBody}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error processing message: {ex.Message}");
        }
    }
    
    public async Task SendStompMessage(string content)
    {
        if (websocket?.State != WebSocketState.Open || !isConnected)
        {
            Debug.LogError("WebSocket not connected. Cannot send message.");
            return;
        }
        
        var sessionId = GameSessionController.GetSessionData().sessionId;
        if (sessionId == 0)
        {
            Debug.LogError("No active game session. Cannot send message.");
            return;
        }
        
        var chatMessage = new ChatMessage
        {
            content = content,
            sender = UserInfoController.GetUsername(),
            type = "CHAT",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        
        var json = JsonUtility.ToJson(chatMessage);
        var destination = $"/app/chat.sendMessage/{sessionId}";
        
        await SendStompFrame(destination, json);
    }
    
    public async Task UpdatePlayerState(PlayerGameInfo playerInfo)
    {
        if (websocket?.State != WebSocketState.Open || !isConnected)
        {
            Debug.LogError("WebSocket not connected. Cannot update player state.");
            return;
        }
        
        var sessionId = GameSessionController.GetSessionData().sessionId;
        if (sessionId == 0)
        {
            Debug.LogError("No active game session. Cannot update player state.");
            return;
        }
        
        var json = JsonUtility.ToJson(playerInfo);
        var destination = $"/app/game.update/{sessionId}";
        
        await SendStompFrame(destination, json);
    }
    
    public async Task FireProjectile(ProjectileEvent projectileEvent)
    {
        if (websocket?.State != WebSocketState.Open || !isConnected)
        {
            Debug.LogError("WebSocket not connected. Cannot fire projectile.");
            return;
        }
        
        var sessionId = GameSessionController.GetSessionData().sessionId;
        if (sessionId == 0)
        {
            Debug.LogError("No active game session. Cannot fire projectile.");
            return;
        }
        
        var json = JsonUtility.ToJson(projectileEvent);
        var destination = $"/app/game.fire/{sessionId}";
        
        await SendStompFrame(destination, json);
    }
    
    private async Task SendStompFrame(string destination, string jsonBody)
    {
        var stompMessage = "SEND\n" +
                         $"destination:{destination}\n" +
                         "content-type:application/json\n\n" +
                         $"{jsonBody}\0";
        
        await websocket.SendText(stompMessage);
    }
    
    private async Task SendStompConnect()
    {
        await websocket.SendText(STOMP_CONNECT_FRAME);
    }
    
    private async Task SubscribeToTopic(string sessionId, string topic)
    {
        var destination = $"/topic/{topic}/{sessionId}";
        
        var subscribeFrame = "SUBSCRIBE\n" +  
                           $"id:{SUBSCRIPTION_ID}\n" + 
                           $"destination:{destination}\n\n\0";
        
        await websocket.SendText(subscribeFrame);
        Debug.Log($"Subscribed to topic: {destination}");
    }

    private async Task DisconnectFromServer()
    {
        if (isConnected && websocket?.State == WebSocketState.Open)
        {
            await websocket.SendText(STOMP_DISCONNECT_FRAME);
        }
        
        if (websocket != null)
        {
            await websocket.Close();
            websocket = null;
            isConnected = false;
        }
    }

    private async Task JoinChat(string sessionId, string username)
    {
        if (websocket?.State != WebSocketState.Open || !isConnected)
        {
            Debug.LogError("WebSocket not connected. Cannot join chat.");
            return;
        }
        
        var chatMessage = new ChatMessage
        {
            content = $"{username} joined the chat",
            sender = username,
            type = "JOIN",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        
        var json = JsonUtility.ToJson(chatMessage);
        var destination = $"/app/chat.join/{sessionId}";
        
        await SendStompFrame(destination, json);
    }

    private async Task JoinGame(string sessionId, string username)
    {
        if (websocket?.State != WebSocketState.Open || !isConnected)
        {
            Debug.LogError("WebSocket not connected. Cannot join game.");
            return;
        }
        
        var gameEvent = new GameEvent
        {
            eventType = "JOIN",
            playerId = UserInfoController.GetUsername(),
            playerName = username,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        
        var json = JsonUtility.ToJson(gameEvent);
        var destination = $"/app/game.join/{sessionId}";
        
        await SendStompFrame(destination, json);
    }

    private void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
        #endif
    }
    
    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await DisconnectFromServer();
        }
    }
}