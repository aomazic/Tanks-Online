// csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameRoomsPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private int pageSize = 10;

    private List<GameSession> roomsData = new List<GameSession>();
    private int currentPage = 0;
    private bool hasMoreData = true;
    
    private IWebClient webClient;
    private DynamicScrollView scrollView;

    public void SetWebClient(IWebClient client)
    {
        webClient = client;
    }

    private void Start()
    {
        // If no custom client was injected, fallback to WebClient.Instance.
        webClient ??= WebClient.Instance;

        scrollView = GetComponentInChildren<DynamicScrollView>();
        scrollView.OnRoomSelected += HandleRoomItemClick;
        
        refreshButton.onClick.AddListener(RefreshRooms);
    }

    private void OnEnable()
    {
        LoadData();
    }

    private void LoadData()
    {
        currentPage = 0;
        hasMoreData = true;
        roomsData.Clear();
        StartCoroutine(FetchCoroutine());
    }

    private void RefreshRooms()
    {
        LoadData();
    }

    private IEnumerator FetchCoroutine()
    {
        loadingIndicator.SetActive(true);

        yield return StartCoroutine(webClient.GetWaitingGameSessions(
            currentPage,
            pageSize,
            (success, response) =>
            {
                if (success)
                {
                    var responseData = JsonUtility.FromJson<GameSessionResponse>(response);
                    roomsData.AddRange(responseData.content);
                    currentPage++;
                    hasMoreData = !responseData.last;
                    scrollView.FillData(roomsData);
                }
                else
                {
                    Debug.LogError($"Failed to load rooms: {response}");
                }
            }
        ));

        loadingIndicator.SetActive(false);
    }

    private void HandleRoomItemClick(int index)
    {
        Debug.Log($"Clicked room item: {index}");
        
        if (index < 0 || index >= roomsData.Count)
        {
            return;
        }
        
        var selectedRoom = roomsData[index];

        if (!string.IsNullOrEmpty(selectedRoom.password))
        {
            ShowPasswordPrompt(selectedRoom);
        }
        else
        {
            JoinRoom(selectedRoom);
        }
    }

    private void ShowPasswordPrompt(GameSession room)
    {
        // Implement password input UI
        Debug.Log($"Password required for room: {room.name}");
        
        // For now, let's just join the room directly
        // In a real implementation, you would show a UI for password input
        JoinRoom(room);
    }

    private void JoinRoom(GameSession room)
    {
        Debug.Log($"Joining room: {room.name}");
        
        // Save the game session data
        GameSessionController.SaveGameSession(room);
        
        // Load the game scene
        SceneManager.LoadScene("TD Test");
    }

    private void OnDestroy()
    {
        if (scrollView != null)
            scrollView.OnRoomSelected -= HandleRoomItemClick;
        
        if (refreshButton != null)
            refreshButton.onClick.RemoveAllListeners();
    }
}