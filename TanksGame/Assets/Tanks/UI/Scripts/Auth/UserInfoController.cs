using UnityEngine;

using UnityEngine;

public class UserInfoController : MonoBehaviour
{
    [SerializeField] private UserInfo userInfoAsset;
    
    private static UserInfo userInfoInstance;
    
    private void Awake()
    {
        if (!userInfoInstance)
        {
            userInfoInstance = userInfoAsset;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Save complete user data from API response
    public static void SaveUserData(User userData)
    {
        if (!userInfoInstance)
        {
            Debug.LogError("UserInfo instance not initialized!");
            return;
        }
        
        userInfoInstance.Token = userData.token;
        userInfoInstance.Username = userData.username;
        userInfoInstance.UserId = userData.id;
    }
    
    public static void SaveToken(string token)
    {
        if (!userInfoInstance)
        {
            Debug.LogError("UserInfo instance not initialized!");
            return;
        }
        
        userInfoInstance.Token = token;
    }
    
    public static string GetUsername()
    {
        return userInfoInstance.Username;
    }

    public static string GetToken()
    {
        return userInfoInstance.Token;
    }
    
    public static void ClearUserData()
    {
        if (!userInfoInstance)
        {
            Debug.LogError("UserInfo instance not initialized!");
            return;
        }
        
        userInfoInstance.Clear();
    }
    
    public static bool IsUserAuthenticated()
    {
        return userInfoInstance && userInfoInstance.IsAuthenticated();
    }
}

