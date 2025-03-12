using System;
using UnityEngine;

public class ChatMessageUI : MonoBehaviour
{
    private TMPro.TextMeshProUGUI messageText;
    private void Awake()
    {
        messageText = GetComponent<TMPro.TextMeshProUGUI>();
    }
    
    public void SetMessage(ChatMessage message)
    {
        // Format timestamp
        var messageTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
            .AddMilliseconds(message.timestamp);
        var timeString = messageTime.ToLocalTime().ToString("HH:mm");
    
        // Format text based on message type
        string formattedText;
    
        switch (message.type)
        {
            case "SYSTEM":
                formattedText = $"<color=#888888>[{timeString}] {message.content}</color>";
                break;
            
            case "JOIN":
                formattedText = $"<color=#6495ED>[{timeString}] {message.content}</color>";
                break;
            
            default:
                var isLocalUser = message.sender == UserInfoController.GetUsername();
                var nameColor = isLocalUser ? "#4CAF50" : "#FF8C00";
            
                formattedText = $"[{timeString}] <color={nameColor}><b>{message.sender}</b></color>: {message.content}";
                break;
        }
    
        messageText.text = formattedText;
    }
}
