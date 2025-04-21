using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    [Header("Chat Input")]
    [SerializeField] private TMP_InputField chatInputField;
    
    [Header("Message Prefabs")]
    [SerializeField] private GameObject messagePrefab;
    
    [Header("Chat Display")]
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private Transform chatContentTransform;
    
    private WebSocketController webSocketController;

    private void Awake() {
        webSocketController = WebSocketController.Instance;
    }

    private void OnEnable()
    {
        // TODO: move this to another place later
        _ = webSocketController.ConnectToServer();
        
        CleanUpOldMessages();
        
        webSocketController.OnChatMessageReceived += HandleChatMessage;
        webSocketController.OnConnectionOpened += HandleConnectionOpened;
        webSocketController.OnConnectionError += HandleConnectionError;
        webSocketController.OnConnectionClosed += HandleConnectionClosed;

        chatInputField.onSubmit.AddListener(OnInputSubmitWrapper);
    }
    
    private void OnDisable()
    {
        webSocketController.OnChatMessageReceived -= HandleChatMessage;
        webSocketController.OnConnectionOpened -= HandleConnectionOpened;
        webSocketController.OnConnectionError -= HandleConnectionError;
        webSocketController.OnConnectionClosed -= HandleConnectionClosed;

        chatInputField.onSubmit.RemoveListener(OnInputSubmitWrapper);
    }
    
    private void OnInputSubmitWrapper(string text)
    {
        _ = OnInputSubmit(text).ContinueWith(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"Error in OnInputSubmit: {task.Exception}");
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
    
    private async Task OnInputSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        await SendMessageToChat(text);
        chatInputField.text = "";
        chatInputField.ActivateInputField();
    }

    private void HandleChatMessage(ChatMessage message)
    {
       Debug.Log(message.content);
       DisplayMessage(message);
    }
    
    private void HandleConnectionOpened()
    {
        DisplaySystemMessage("Connected to chat");
    }
    
    private void HandleConnectionError(string error)
    {
        DisplaySystemMessage($"Connection error: {error}");
    }

    private void HandleConnectionClosed()
    {
        DisplaySystemMessage("Disconnected from chat");
    }

    private void DisplaySystemMessage(string message)
    {
        var systemMessage = new ChatMessage
        {
            content = message,
            sender = "System",
            type = "SYSTEM",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        
        DisplayMessage(systemMessage);
    }
    
    private void CleanUpOldMessages()
    {
        foreach (Transform child in chatContentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisplayMessage(ChatMessage message)
    {
        if (!messagePrefab || !chatContentTransform)
        {
            Debug.LogWarning("Cannot display message: missing message prefab or content transform");
            return;
        }
    
        var messageObj = Instantiate(messagePrefab, chatContentTransform);
        var chatMessageUI = messageObj.GetComponent<ChatMessageUI>();
    
        if (!chatMessageUI)
        {
            Debug.LogWarning("Message prefab does not contain ChatMessageUI component");
            return;
        }
    
        chatMessageUI.SetMessage(message);
        ScrollChatToBottom();
    }
    
    private void ScrollChatToBottom()
    {
        if (chatScrollRect)
        {
           return;
        }
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }

    private async Task SendMessageToChat(string message)
    {
        await webSocketController.SendStompMessage(message);
    }
}