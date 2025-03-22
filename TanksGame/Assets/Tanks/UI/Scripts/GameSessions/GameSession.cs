using UnityEngine.Serialization;

[System.Serializable]
public class GameSession
{
    public string name;
    public string password;
    public long id;
    public int maxPlayers;
    public int currentPlayers;
}