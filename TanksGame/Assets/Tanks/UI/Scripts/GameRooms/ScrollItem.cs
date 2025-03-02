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
    
    public event System.Action<int> OnClick;

    private Button button;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }

    public void SetData(GameSession data)
    {
        roomName.text = data.name;
        playerCount.text = $"{data.currentPlayers}/{data.maxPlayers}";
        passwordIcon.gameObject.SetActive(!string.IsNullOrEmpty(data.password));
    }
}
