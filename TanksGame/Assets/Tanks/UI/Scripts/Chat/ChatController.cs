using System;
using UnityEngine;

public class ChatController : MonoBehaviour
{
    private WebSocketController webSocketController;
    
    private void Awake()
    {
        webSocketController = WebSocketController.Instance;
    }

    private void OnEnable()
    {
        webSocketController.ConnectToChat();
    }
}
