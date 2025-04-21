using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Serialization;

public class ConsoleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField consoleInput;
    [SerializeField] private TMP_Text infoTextRef;
    [SerializeField] private TMP_Text terminalText;
    
    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioClip clickSound;
    
    private Coroutine typingCoroutine;
    private ConsoleState currentState = ConsoleState.AuthMain;
    
    private User user;
    
    private string tempPassword;
    private string tempEmail;
    
    private WebClient webClient;
    
    private void Start()
    {
        terminalText.text = TerminalTexts.GetTerminalText(currentState);
        TypeText("Welcome Commander! Type '1', '2', or '3' to proceed. To exit, type 'exit'", infoTextRef);
        consoleInput.ActivateInputField();
        
        webClient = WebClient.Instance;
    }

    public void OnSubmit()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
        var userInput = consoleInput.text.Trim().ToLower();
        consoleInput.text = "";

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        switch (currentState)
        {
            case ConsoleState.AuthMain:
                HandleMainMenuInput(userInput);
                break;
            case ConsoleState.LoginUsername:
                HandleUsernameInput(userInput);
                break;
            case ConsoleState.LoginPassword:
                HandlePasswordInput(userInput);
                break;
            case ConsoleState.RegisterEmail:
                HandleEmailInput(userInput);
                break;
            case ConsoleState.RegisterUserName:
                HandleRegisterUsernameInput(userInput);
                break;
            case ConsoleState.RegisterPassword:
                HandleRegisterPasswordInput(userInput);
                break;
        }

        consoleInput.ActivateInputField();
    }

    private void HandleMainMenuInput(string userInput)
    {
        switch (userInput)
        {
            case "1":
                currentState = ConsoleState.LoginUsername;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Enter your username: ", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
                break;
            case "2":
                currentState = ConsoleState.RegisterEmail;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Enter your e-mail. Type 'back' to return or 'exit' to quit.", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.EmailAddress;
                break;
            case "3":
                currentState = ConsoleState.Guest;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                HandleGuestRegister();
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
                break;
            case "exit":
                ExitCase();
                break;
            default:
                TypeText("Invalid Command. ", infoTextRef, "Type '1', '2', or '3' to proceed. To exit, type 'exit'");
                break;
        }
        
        terminalText.text = TerminalTexts.GetTerminalText(currentState);
    }

    private void HandleUsernameInput(string userInput)
    {
        switch (userInput)
        {
            case "back":
                currentState = ConsoleState.AuthMain;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
                break;
            case "exit":
                ExitCase();
                break;
            default:
                currentState = ConsoleState.LoginPassword;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                user.username = userInput;
                TypeText("Enter your password: ", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.Password;
                break;
        }
    }
    
    private void HandlePasswordInput(string userInput)
    {
        switch (userInput)
        {
            case "back":
                currentState = ConsoleState.AuthMain;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
                break;
            case "exit":
                ExitCase();
                break;
            default:
                tempPassword = userInput;
                StartCoroutine(webClient.Login(user.username, tempPassword, (success, response) =>
                {
                    if (success)
                    {
                        user.token = response;
                        currentState = ConsoleState.Entering;
                        TypeText("Login successful!", infoTextRef);
                        EnterGame();
                    }
                    else
                    {
                        currentState = ConsoleState.LoginUsername;
                        terminalText.text = TerminalTexts.GetTerminalText(currentState);
                        TypeText(response, infoTextRef, "Login failed! Enter you username");
                        consoleInput.contentType = TMP_InputField.ContentType.Standard;
                    }
                }));
                break;
        }
    }
    
    private void HandleEmailInput(string userInput)
    {
        switch (userInput)
        {
            case "back":
                currentState = ConsoleState.AuthMain;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
                break;
            case "exit":
                ExitCase();
                break;
            default:
                tempEmail = userInput;
                currentState = ConsoleState.RegisterUserName;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Chose your username: ", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
                break;
        }
    }
    
    private void HandleRegisterUsernameInput(string userInput)
    {
        switch (userInput)
        {
            case "back":
                currentState = ConsoleState.AuthMain;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
                break;
            case "exit":
                ExitCase();
                break;
            default:
                user.username = userInput;
                StartCoroutine(webClient.CheckUsername(user.username, (success, response) =>
                {
                    if (success)
                    {
                        currentState = ConsoleState.RegisterPassword;
                        terminalText.text = TerminalTexts.GetTerminalText(currentState);
                        TypeText("Chose your password: ", infoTextRef);
                        consoleInput.contentType = TMP_InputField.ContentType.Password;
                    }
                    else
                    {
                        currentState = ConsoleState.RegisterUserName;
                        terminalText.text = TerminalTexts.GetTerminalText(currentState);
                        TypeText(response, infoTextRef,"Username is taken! Enter a new username:");
                        consoleInput.contentType = TMP_InputField.ContentType.Standard;
                    }
                }));
                break;
        }
    }
    
    private void HandleRegisterPasswordInput(string userInput)
    {
        terminalText.text = TerminalTexts.GetTerminalText(currentState);
        switch (userInput)
        {
            case "back":
                currentState = ConsoleState.AuthMain;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
                break;
            case "exit":
                ExitCase();
                break;
            default:
                tempPassword = userInput;
                StartCoroutine(webClient.Register(user.username, tempPassword, tempEmail, (success, userResponse) =>
                {
                    if (success)
                    {
                        user = userResponse;
                        currentState = ConsoleState.Entering;
                        TypeText("Registration successful!", infoTextRef);
                        EnterGame();
                    }
                    else
                    {
                        currentState = ConsoleState.RegisterUserName;
                        terminalText.text = TerminalTexts.GetTerminalText(currentState);
                        TypeText(userResponse.ToString(), infoTextRef, "Enter you username:");
                        consoleInput.contentType = TMP_InputField.ContentType.Standard;
                    }
                }));
                break;
        }
    }
    
    private void HandleGuestRegister()
    {
        consoleInput.text = "";
        
        StartCoroutine(webClient.GuestRegister((success, userResponse) =>
        {
            if (success)
            {
                user = userResponse; 
                currentState = ConsoleState.Entering;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Guest account created!", infoTextRef);
                EnterGame();
            }
            else
            {
                TypeText(userResponse.ToString(), infoTextRef, "Guest account creation failed!");
                currentState = ConsoleState.AuthMain;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
            }
        }));
        currentState = ConsoleState.AuthMain;
        terminalText.text = TerminalTexts.GetTerminalText(currentState);
    }
    
    private void EnterGame()
    {
        UserInfoController.SaveUserData(user);
        StartCoroutine(webClient.UpdateUserStatus(user.username, UserStatus.ONLINE, (success, response) =>
        {
            if (success)
            {
                StartCoroutine(InitiateEnteringGame("Entering the game...", infoTextRef, SceneController.LoadInitializing));
            }
            else
            {
                TypeText(response, infoTextRef, "Failed to enter the game!");
                currentState = ConsoleState.AuthMain;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                consoleInput.contentType = TMP_InputField.ContentType.Standard;
            }
        }));
    }
    
    private IEnumerator InitiateEnteringGame(string text, TMP_Text textComponent, System.Action loadSceneAction)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        yield return TypeTextCoroutine(text, textComponent);
        loadSceneAction();
    }
    
    private void TypeText(string text, TMP_Text textComponent, string nextText = null)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        typingCoroutine = StartCoroutine(TypeTextCoroutine(text, textComponent, nextText));
    }

    private IEnumerator TypeTextCoroutine(string text, TMP_Text textComponent, string nextText = null)
    {
        textComponent.text = "";
        foreach (var c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        if (audioSource != null && typingSound != null)
        {
            audioSource.Stop();
        }
        if (!string.IsNullOrEmpty(nextText))
        {
            yield return StartCoroutine(TypeTextCoroutine(nextText, textComponent));
        }
    }
    
    private void ExitCase()
    {
        TypeText("Exiting... Goodbye Commander!", infoTextRef);
        StopCoroutine(webClient.UpdateUserStatus(user.username, UserStatus.OFFLINE, (success, response) =>
        {
            if (!success)
            {
                Debug.LogError(response);
            }
        }));
        
        Application.Quit();
    }
}