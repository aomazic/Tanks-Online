using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameRoomItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private Image passwordIcon;
    
    private int itemIndex;
    public RectTransform RectTransform { get; private set; }
    
    public event System.Action<int> OnClick;

    private Button button;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        OnClick?.Invoke(itemIndex);
    }

    public void SetData(GameSession data, int index)
    {
        roomName.text = data.name;
        playerCount.text = $"{data.currentPlayers}/{data.maxPlayers}";
        passwordIcon.gameObject.SetActive(!string.IsNullOrEmpty(data.password));
        itemIndex = index;
    }
}
