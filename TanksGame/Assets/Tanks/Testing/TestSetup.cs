using UnityEngine;

public class TestSetup : MonoBehaviour
{
    [SerializeField] private GameRoomsPanel gameRoomsPanel; 
    
    private MockWebClient mockWebClient;

    private void Awake()
    {
        mockWebClient = GetComponent<MockWebClient>();
        gameRoomsPanel.SetWebClient(mockWebClient);
    }
}
