using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameRoomsPanel : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Button refreshButton;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private int pageSize = 10;

    private List<GameSession> roomsData = new List<GameSession>();
    private int currentPage = 0;
    private bool hasMoreData = true;
    
    private WebClient webClient;
    private RecyclableScrollView scrollView;
    
    private void Start()
    {
        webClient = WebClient.Instance;
        scrollView = GetComponentInChildren<RecyclableScrollView>();
        
        scrollView.OnItemClicked += HandleRoomItemClick;
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
        GameSession selectedRoom = roomsData[index];
        
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
        Debug.Log($"Password required for room: {room.roomName}");
    }

    private void JoinRoom(GameSession room)
    {
        Debug.Log($"Joining room: {room.roomName}");
        // Implement actual join logic
    }

    private void OnDestroy()
    {
        scrollView.OnItemClicked -= HandleRoomItemClick;
        refreshButton.onClick.RemoveAllListeners();
    }
}