using UnityEngine;

[CreateAssetMenu(fileName = "UserInfo", menuName = "Game/UserInfo")]
public class UserInfo : ScriptableObject
{
    [Header("User Authentication")]
    [SerializeField] private User userData;

    public string Token { 
        get => userData?.token ?? string.Empty; 
        set => InitUserDataIfNeeded().token = value; 
    }
    
    public string Username { 
        get => userData?.username ?? string.Empty; 
        set => InitUserDataIfNeeded().username = value; 
    }
    
    public string UserId { 
        get => userData?.id ?? string.Empty; 
        set => InitUserDataIfNeeded().id = value; 
    }

    private User InitUserDataIfNeeded()
    {
        if (userData == null)
            userData = new User();
        return userData;
    }

    public void UpdateFromUser(User user)
    {
        if (user == null) return;
        
        userData = new User
        {
            id = user.id,
            username = user.username,
            token = user.token
        };
    }

    public void Clear()
    {
        userData = new User();
    }

    public bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(Token);
    }
}