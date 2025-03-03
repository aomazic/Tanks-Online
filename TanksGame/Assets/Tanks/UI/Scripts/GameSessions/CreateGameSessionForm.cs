using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class CreateRoomForm : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button createButton;
    
    [Header("Settings")]
    [SerializeField] private int minCharLength = 3;
    [SerializeField] private int maxCharLength = 16;
    
    [Header("Dependencies")]
    [SerializeField] private GameSessionData sessionData;
    
    private GameSessionData gameSessionData;
    private WebClient webClient;
    
    void Start()
    {
        createButton.onClick.AddListener(OnCreateButtonClicked);
        
        nameInputField.onValueChanged.AddListener(ValidateInputs);
        passwordInputField.onValueChanged.AddListener(ValidateInputs);
        
        ValidateInputs(nameInputField.text);
        ValidateInputs(passwordInputField.text);
        
        passwordInputField.contentType = TMP_InputField.ContentType.Password;
        
        webClient = WebClient.Instance;
    }

    private void ValidateInputs(string _)
    {
        var isValid = !string.IsNullOrEmpty(nameInputField.text) && 
                      nameInputField.text.Length >= minCharLength && 
                      nameInputField.text.Length <= maxCharLength;
                      
        createButton.interactable = isValid;
    }

    private void OnCreateButtonClicked()
    {
        var roomName = nameInputField.text;
        var password = passwordInputField.text;
        
        
        // Show loading indicator if you have one
        createButton.interactable = false;
    
        StartCoroutine(webClient.CreateGameSession(roomName, password, (success, gameSession) => {
            if (success && gameSession != null)
            {
                // Store the session data in scriptable object
                gameSessionData.sessionId = gameSession.sessionId;
                gameSessionData.sessionName = gameSession.name;
            
                Debug.Log($"Game session created successfully: {gameSession.sessionId}");
            
                ClearForm();
                gameObject.SetActive(false);
            
                // Optional: Notify UI manager or game manager about successful creation
                // GameManager.Instance.OnGameSessionCreated(gameSession);
            }
            else
            {
                Debug.LogError("Failed to create game session");
                // Re-enable button on failure
                createButton.interactable = true;
                // Show error message to user if you have UI for it
            }
        }));
    }
    
    private void ClearForm()
    {
        nameInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
    }

    void OnDestroy()
    {
        if (createButton != null)
            createButton.onClick.RemoveListener(OnCreateButtonClicked);
        if (nameInputField != null)
            nameInputField.onValueChanged.RemoveListener(ValidateInputs);
    }
}