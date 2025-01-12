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
    [SerializeField] private WebClient webClient;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] [TextArea(5,10)] private string defaultTerminalText = "";

    private Coroutine typingCoroutine;
    private ConsoleState currentState = ConsoleState.MainMenu;

    
    private string tempPassword = "";
    private string tempUsername = "";
    private string tempEmail = "";
    
    private void Start()
    {
        terminalText.text = TerminalTexts.GetTerminalText(currentState);
        TypeText("Welcome Commander! Type '1', '2', or '3' to proceed. To exit, type 'exit'", infoTextRef);
        consoleInput.ActivateInputField();
    }

    public void OnSubmit()
    {
        var userInput = consoleInput.text.Trim().ToLower();
        consoleInput.text = "";

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        switch (currentState)
        {
            case ConsoleState.MainMenu:
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
                break;
            case "2":
                currentState = ConsoleState.RegisterEmail;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Enter your e-mail. Type 'back' to return or 'exit' to quit.", infoTextRef);
                break;
            case "3":
                currentState = ConsoleState.Guest;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                HandleGuestRegister();
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
                currentState = ConsoleState.MainMenu;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                break;
            case "exit":
                ExitCase();
                break;
            default:
                currentState = ConsoleState.LoginPassword;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                tempUsername = userInput;
                TypeText("Enter your password: ", infoTextRef);
                break;
        }
    }
    
    private void HandlePasswordInput(string userInput)
    {
        switch (userInput)
        {
            case "back":
                currentState = ConsoleState.MainMenu;
                TypeText("Choose Option to Enter System", infoTextRef);
                break;
            case "exit":
                ExitCase();
                break;
            default:
                tempPassword = userInput;
                StartCoroutine(webClient.Login(tempUsername, tempPassword, (success, response) =>
                {
                    if (success)
                    {
                        TypeText("Login successful!", infoTextRef);
                        EnterGame();
                    }
                    else
                    {
                        currentState = ConsoleState.LoginUsername;
                        terminalText.text = TerminalTexts.GetTerminalText(currentState);
                        TypeText(response, infoTextRef, "Login failed! Enter you username");
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
                currentState = ConsoleState.MainMenu;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                break;
            case "exit":
                ExitCase();
                break;
            default:
                tempEmail = userInput;
                currentState = ConsoleState.RegisterUserName;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Chose your username: ", infoTextRef);
                break;
        }
    }
    
    private void HandleRegisterUsernameInput(string userInput)
    {
        switch (userInput)
        {
            case "back":
                currentState = ConsoleState.MainMenu;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                break;
            case "exit":
                ExitCase();
                break;
            default:
                tempUsername = userInput;
                StartCoroutine(webClient.CheckUsername(tempUsername, (success, response) =>
                {
                    if (success)
                    {
                        currentState = ConsoleState.RegisterPassword;
                        terminalText.text = TerminalTexts.GetTerminalText(currentState);
                        TypeText("Chose your password: ", infoTextRef);
                    }
                    else
                    {
                        currentState = ConsoleState.RegisterUserName;
                        terminalText.text = TerminalTexts.GetTerminalText(currentState);
                        TypeText(response, infoTextRef,"Username is taken! Enter a new username:");
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
                currentState = ConsoleState.MainMenu;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Choose Option to Enter System", infoTextRef);
                break;
            case "exit":
                ExitCase();
                break;
            default:
                tempPassword = userInput;
                StartCoroutine(webClient.Register(tempUsername, tempPassword, tempEmail, (success, response) =>
                {
                    if (success)
                    {
                        TypeText("Registration successful!", infoTextRef);
                        EnterGame();
                    }
                    else
                    {
                        currentState = ConsoleState.RegisterUserName;
                        terminalText.text = TerminalTexts.GetTerminalText(currentState);
                        TypeText(response, infoTextRef, "Enter you username:");
                    }
                }));
                break;
        }
    }
    
    private void HandleGuestRegister()
    {
        consoleInput.text = "";
        StartCoroutine(webClient.GuestRegister((success, response) =>
        {
            if (success)
            {
                TokenManager.SaveToken(response);
                currentState = ConsoleState.Entering;
                terminalText.text = TerminalTexts.GetTerminalText(currentState);
                TypeText("Guest account created!", infoTextRef);
                EnterGame();
            }
            else
            {
                TypeText(response, infoTextRef,"Guest account creation failed!");
            }
        }));
        currentState = ConsoleState.MainMenu;
        terminalText.text = TerminalTexts.GetTerminalText(currentState);
    }
    
    private void EnterGame()
    {
        StopCoroutine(webClient.updateUserStatus(tempUsername, UserStatus.ONLINE, (success, response) =>
        {
            if (success)
            {
                TypeText("Entering the game...", infoTextRef);
                // Load the actual main menu scene
            }
            else
            {
                TypeText(response, infoTextRef, "Failed to enter the game!");
            }
        }));
    }
    
    
    private void TypeText(string text, TMP_Text textComponent, string nextText = null)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
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

        if (!string.IsNullOrEmpty(nextText))
        {
            yield return StartCoroutine(TypeTextCoroutine(nextText, textComponent));
        }
    }
    
    private void ExitCase()
    {
        TypeText("Exiting... Goodbye Commander!", infoTextRef);
        Application.Quit();
    }
}
