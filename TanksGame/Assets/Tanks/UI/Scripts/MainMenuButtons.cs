using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button findGameButton;
    [SerializeField] private GameObject findGamePanel;
    [SerializeField] private GameObject mainPanel; // Example for another UI object
    

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
}