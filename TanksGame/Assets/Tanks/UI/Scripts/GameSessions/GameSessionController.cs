using System;
using UnityEngine;

public class GameSessionController : MonoBehaviour
{
    [SerializeField] private GameSessionData sessionData;
    
    private static GameSessionData gameSessionDataInstance;
    
    private void Awake()
    {
        if (!gameSessionDataInstance)
        {
            gameSessionDataInstance = sessionData;
            // Make this GameObject a root object to ensure DontDestroyOnLoad works properly
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }    
    public static void SaveGameSession(GameSession gameSession)
    {
        if (!gameSessionDataInstance)
        {
            Debug.LogError("GameSessionData asset not assigned!");
            return;
        }
        
        gameSessionDataInstance.sessionId = gameSession.id;
        gameSessionDataInstance.sessionName = gameSession.name;
        gameSessionDataInstance.sessionPassword = gameSession.password;
    }
    
    public static GameSessionData GetSessionData()
    {
        return gameSessionDataInstance;
    }

    public long GetSessionId()
    {
        return gameSessionDataInstance.sessionId;
    }

    public void ClearSessionData()
    {
        if (gameSessionDataInstance != null)
        {
            gameSessionDataInstance.Clear();
        }
    }
}