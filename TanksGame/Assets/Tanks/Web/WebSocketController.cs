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
    public event Action<ChatMessage> OnUserJoined;
    public event Action OnConnectionOpened;
    public event Action<string> OnConnectionError;
    public event Action OnConnectionClosed;
    
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
    
    public async void ConnectToChat()
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

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket connection opened");
            OnConnectionOpened?.Invoke();
            
            var sessionId = GameSessionController.GetSessionData().sessionId;
            if (!string.IsNullOrEmpty(sessionId))
            {
                JoinChat(sessionId, UserInfoController.GetUsername());
            }
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"WebSocket Error: {e}");
            OnConnectionError?.Invoke(e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket connection closed");
            OnConnectionClosed?.Invoke();
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log($"WebSocket message received: {message}");
            
            var chatMessage = JsonUtility.FromJson<ChatMessage>(message);
            
            if (chatMessage.type == "JOIN")
            {
                OnUserJoined?.Invoke(chatMessage);
            }
            else
            {
                OnChatMessageReceived?.Invoke(chatMessage);
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
    
    public async Task DisconnectFromChat()
    {
        if (websocket?.State == WebSocketState.Open)
        {
            await websocket.Close();
            websocket = null;
        }
    }
    
    public async void SendMessage(string content)
    {
        if (websocket?.State != WebSocketState.Open)
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
        var stompMessage = $"SEND\ndestination:{destination}\ncontent-type:application/json\n\n{json}\0";
        
        await websocket.SendText(stompMessage);
    }
    
    public async void JoinChat(string sessionId, string username)
    {
        if (websocket?.State != WebSocketState.Open)
        {
            Debug.LogError("WebSocket not connected. Cannot join chat.");
            return;
        }
        
        ChatMessage chatMessage = new ChatMessage
        {
            content = $"{username} joined the chat",
            sender = username,
            type = "JOIN",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        
        var json = JsonUtility.ToJson(chatMessage);
        var destination = $"/app/chat.join/{sessionId}";
        
        // Create STOMP-formatted message
        var stompMessage = $"SEND\ndestination:{destination}\ncontent-type:application/json\n\n{json}\0";
        
        await websocket.SendText(stompMessage);
    }
    
    void Update()
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
