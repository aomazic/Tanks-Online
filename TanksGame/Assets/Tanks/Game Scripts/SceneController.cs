using UnityEngine;

public class SceneController : MonoBehaviour
{
    private static SceneController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void LoadAuthScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Auth");
    }

    public static void LoadInitializing()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Initializing");
    }
    
    public static void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
