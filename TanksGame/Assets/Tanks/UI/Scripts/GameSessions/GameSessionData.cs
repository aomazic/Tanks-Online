using UnityEngine;

[CreateAssetMenu(fileName = "GameSessionData", menuName = "Game/Game Session Data")]
public class GameSessionData : ScriptableObject
{
    public string sessionId;
    public string sessionName;
    public string sessionPassword;
    
    public void Clear()
    {
        sessionId = string.Empty;
        sessionName = string.Empty;
        sessionPassword = string.Empty;
    }
}