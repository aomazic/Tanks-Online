using UnityEngine;

using UnityEngine;

public class UserInfoManager : MonoBehaviour
{
    [SerializeField] private UserInfo userInfoAsset;
    
    private static UserInfo userInfoInstance;
    public static UserInfoManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            userInfoInstance = userInfoAsset;
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
    
    public static void SaveUserName(string username)
    {
        if (!userInfoInstance)
        {
            Debug.LogError("UserInfo instance not initialized!");
            return;
        }
        
        userInfoInstance.Username = username;
    }
    
    public static void SaveUserId(string userId)
    {
        if (!userInfoInstance)
        {
            Debug.LogError("UserInfo instance not initialized!");
            return;
        }
        
        userInfoInstance.UserId = userId;
    }
    
    public static UserInfo GetUserInfo()
    {
        return userInfoInstance;
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

