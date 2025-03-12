using System;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;
public class WebSocketController : MonoBehaviour
{
    private const string BaseUrl = "ws://localhost:8080/ws/websocket";
    private WebSocket websocket;
    
    public static WebSocketController Instance { get; private set; }
    
    // Callback event for receiving messages
    public event Action<ChatMessage> OnChatMessageReceived;
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
    public async Task ConnectToChat()
    {
        if (websocket != null)
        {
            await DisconnectFromChat();
        }

        if (!UserInfoController.IsUserAuthenticated())
        {
            Debug.LogError("User not authenticated. Cannot connect to chat.");
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
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }
    
            await SubscribeToTopic(sessionId);
            await JoinChat(sessionId, UserInfoController.GetUsername());
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
                ProcessChatMessage(body);
                return;
            }

            // Try to process as direct JSON (fallback)
            try
            {
                ProcessChatMessage(message);
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
    
    private void ProcessChatMessage(string jsonMessage)
    {
        try
        { 
            OnChatMessageReceived?.Invoke(JsonUtility.FromJson<ChatMessage>(jsonMessage));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error processing chat message: {ex.Message}");
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
        if (string.IsNullOrEmpty(sessionId))
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
        
        // Create STOMP-formatted message
        var stompMessage = "SEND\n" +
                           $"destination:{destination}\n" +
                           "content-type:application/json\n\n" +
                           $"{json}\0";
        
        await websocket.SendText(stompMessage);
    }
    
    private async Task SendStompConnect()
    {
        await websocket.SendText(STOMP_CONNECT_FRAME);
    }
    
    private async Task SubscribeToTopic(string sessionId)
    {
        var destination = $"/topic/chat/{sessionId}";
        
        var subscribeFrame = "SUBSCRIBE\n" +  
                                   "id:sub-0\n" + 
                                   $"destination:{destination}\n\n\0";
        
        await websocket.SendText(subscribeFrame);
        Debug.Log($"Subscribed to topic: {destination}");
    }
    
    private async Task DisconnectFromChat()
    {
        if (isConnected && websocket?.State == WebSocketState.Open)
        {
            // Send STOMP disconnect frame
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
        
        // Create STOMP-formatted message
        var stompMessage = "SEND\n" +
                           $"destination:{destination}\n" +
                           "content-type:application/json\n\n" +
                           $"{json}\0";
        
        await websocket.SendText(stompMessage);
    }
    
    private void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
        #endif
    }
    
    private void OnApplicationQuit()
    {
        _ = DisconnectFromChat();
    }
}