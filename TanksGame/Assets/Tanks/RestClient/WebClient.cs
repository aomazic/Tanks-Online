using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    private const string BaseUrl = "http://localhost:8080/api";
    
    public IEnumerator Login(string username, string password, System.Action<bool, string> callback)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using var request = UnityWebRequest.Post($"{BaseUrl}/auth/login", form);
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text);
        }
        else
        {
            callback(false, request.error);
        }
    }
    
    public IEnumerator Register(string username, string password, string email, System.Action<bool, string> callback)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("email", email);

        using var request = UnityWebRequest.Post($"{BaseUrl}/auth/register", form);
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text);
        }
        else
        {
            callback(false, request.error);
        }
    }
    
    public IEnumerator CheckUsername(string username, System.Action<bool, string> callback)
    {
        using var request = UnityWebRequest.Get($"{BaseUrl}/auth/check-username/{username}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            bool doesUsernameExist = bool.Parse(request.downloadHandler.text);
            if (doesUsernameExist)
            {
                callback(false, "Username is not available.");
            }
            else
            {
                callback(true, request.downloadHandler.text);
            }
        }
        else
        {
            callback(false, request.error);
        }
    }
    
    public IEnumerator GuestRegister(System.Action<bool, string> callback)
    {
        using var request = UnityWebRequest.Post($"{BaseUrl}/auth/guest-register", new WWWForm());
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text);
        }
        else
        {
            callback(false, request.error);
        }
    }
    
    public IEnumerator updateUserStatus(string username, UserStatus status, System.Action<bool, string> callback)
    {
        var form = new WWWForm();
        form.AddField("status", status.ToString());

        using var request = UnityWebRequest.Post($"/{username}/status", form);
        
        if (!TokenManager.HasToken())
        {
            callback(false, "No token available. Please log in.");
            yield break;
        }
        
        request.SetRequestHeader("Authorization", $"Bearer {TokenManager.GetToken()}");
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text);
        }
        else
        {
            callback(false, request.error);
        }
    }

}
