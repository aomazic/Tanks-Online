using UnityEngine.Serialization;

[System.Serializable]
public class GameSession
{
    public string name;
    public string password;
    public string sessionId;
    public int maxPlayers;
    public int currentPlayers;
}