using UnityEngine;

public static class TokenManager
{
    private const string TokenKey = "auth_token";

    public static void SaveToken(string token)
    {
        PlayerPrefs.SetString(TokenKey, token);
        PlayerPrefs.Save();
    }

    public static string GetToken()
    {
        return PlayerPrefs.GetString(TokenKey, null);
    }

    public static void ClearToken()
    {
        PlayerPrefs.DeleteKey(TokenKey);
    }

    public static bool HasToken()
    {
        return !string.IsNullOrEmpty(GetToken());
    }
}