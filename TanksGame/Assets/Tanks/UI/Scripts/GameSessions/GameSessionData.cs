using UnityEngine;

[CreateAssetMenu(fileName = "GameSessionData", menuName = "Game/Game Session Data")]
public class GameSessionData : ScriptableObject
{
    public long sessionId;
    public string sessionName;
    public string sessionPassword;
    
    public void Clear()
    {
        sessionId = 0;
        sessionName = string.Empty;
        sessionPassword = string.Empty;
    }
}