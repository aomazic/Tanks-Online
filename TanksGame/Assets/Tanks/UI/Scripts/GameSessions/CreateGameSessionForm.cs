using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateRoomForm : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button createButton;
    [SerializeField] private GameObject sessionPanel;
    [SerializeField] private GameObject parentPanel;

    [Header("Settings")]
    [SerializeField] private int minCharLength = 3;
    [SerializeField] private int maxCharLength = 16;

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
        
        createButton.interactable = false;

        StartCoroutine(webClient.CreateGameSession(roomName, password, (success, gameSession) => {
            if (success && gameSession != null)
            {
                GameSessionController.SaveGameSession(gameSession);

                Debug.Log($"Game session created successfully: {gameSession.id}");

                ClearForm();
                parentPanel.SetActive(false);
                sessionPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Failed to create game session");
                
     
                createButton.interactable = true;
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