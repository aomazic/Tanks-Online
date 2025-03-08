[System.Serializable]
public class ChatMessage
{
    public string content;
    public string sender;
    public string type;
    public long timestamp;
    
    public void setSender(string username)
    {
        sender = username;
    }
}