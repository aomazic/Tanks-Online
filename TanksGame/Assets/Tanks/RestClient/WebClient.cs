using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour, IWebClient
{
    private const string BaseUrl = "http://localhost:8080/api";
    
    public static WebClient Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
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
        using var request = UnityWebRequest.Get($"{BaseUrl}/auth/{username}");

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
        using var request = UnityWebRequest.Post($"{BaseUrl}/auth/guest", new WWWForm());
        
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
    
    public IEnumerator UpdateUserStatus(string username, UserStatus status, System.Action<bool, string> callback)
    {
        var form = new WWWForm();
        form.AddField("newStatus", status.ToString());

        using var request = UnityWebRequest.Post($"{BaseUrl}/user/{username}/status", form);

        if (!UserInfoManager.IsUserAuthenticated())
        {
            callback(false, "No token available. Please log in.");
            yield break;
        }

        request.SetRequestHeader("Authorization", $"Bearer {UserInfoManager.GetToken()}");
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

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
    
    public IEnumerator GetWaitingGameSessions(int page, int size, System.Action<bool, string> callback)
    {
        string url = $"{BaseUrl}/game-sessions/status/WAITING?page={page}&size={size}";
        using var request = UnityWebRequest.Get(url);

        if (!UserInfoManager.IsUserAuthenticated())
        {
            callback(false, "No token available. Please log in.");
            yield break;
        }

        request.SetRequestHeader("Authorization", $"Bearer {UserInfoManager.GetToken()}");

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
    
    public IEnumerator CreateGameSession(string name, string password, System.Action<bool, GameSession> callback)
    {
        var form = new WWWForm();
        form.AddField("name", name);
        
        if (!string.IsNullOrEmpty(password))
        {
            form.AddField("password", password);
        }

        using var request = UnityWebRequest.Post($"{BaseUrl}/game-sessions", form);
        
        request.SetRequestHeader("Content-Type", "application/json");
    
        if (!UserInfoManager.IsUserAuthenticated())
        {
            callback(false, null);
            yield break;
        }
    
        request.SetRequestHeader("Authorization", $"Bearer {UserInfoManager.GetToken()}");
    
        // Construct the JSON payload
        string jsonPayload = $"{{\"name\":\"{name}\",\"password\":\"{password}\"}}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            GameSession gameSession = JsonUtility.FromJson<GameSession>(request.downloadHandler.text);
            callback(true, gameSession);
        }
        else
        {
            Debug.LogError($"Create game session failed: {request.error}");
            callback(false, null);
        }
    }
}
