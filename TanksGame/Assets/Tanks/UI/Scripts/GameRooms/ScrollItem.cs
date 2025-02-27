using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScrollItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private Image passwordIcon;
    
    public RectTransform RectTransform { get; private set; }
    public int Index { get; private set; }
    
    public event System.Action<int> OnClick;

    private Button _button;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OnClick?.Invoke(Index));
    }

    public void SetData(int index, GameSession data)
    {
        Index = index;
        roomName.text = data.roomName;
        playerCount.text = $"{data.currentPlayers}/{data.maxPlayers}";
        passwordIcon.gameObject.SetActive(!string.IsNullOrEmpty(data.password));
    }
}
