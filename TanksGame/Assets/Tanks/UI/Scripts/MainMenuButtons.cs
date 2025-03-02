using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button findGameButton;
    [SerializeField] private GameObject findGamePanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject gameRoomPanel;
    

    // Disables other panels and enables the find game panel
    public void HandleFindGame()
    {
        // Disable other UI objects
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
        }
        // Enable the Find Game UI object
        if (findGamePanel != null)
        {
            findGamePanel.SetActive(true);
        }
    }
    
    public void HandleGoBack()
    {
        // Disable the Find Game UI object
        if (findGamePanel != null)
        {
            findGamePanel.SetActive(false);
        }
        // Enable other UI objects
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }
    }
    
    public void HandleGameRooms()
    {
        // Disable other UI objects
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
        }
        // Enable the Game Rooms UI object
        if (gameRoomPanel != null)
        {
            gameRoomPanel.SetActive(true);
        }
    }
}