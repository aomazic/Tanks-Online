using UnityEngine;

public class UserInfoManager
{
    private const string UserNameKey = "username";
    
    public static void SaveUserName(string username)
    {
        PlayerPrefs.SetString(UserNameKey, username);
        PlayerPrefs.Save();
    }
    
    public static string GetUserName()
    {
        return PlayerPrefs.GetString(UserNameKey, null);
    }
    
    public static void ClearUserName()
    {
        PlayerPrefs.DeleteKey(UserNameKey);
    }
    
    public static bool hasUserName()
    {
        return !string.IsNullOrEmpty(GetUserName());
    }
    
}
