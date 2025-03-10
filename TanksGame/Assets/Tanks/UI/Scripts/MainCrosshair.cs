using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCrosshair : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Slider healthSlider;

    [Header("Visuals")]
    [SerializeField] private Color alignmentColor = Color.green;
    [SerializeField] private Color nonAlignmentColor = Color.red;
    
    private RectTransform crossHair;
    private Material cursorMaterial;
    private Image cursorImage;
    private Camera mainCamera;
    private bool isAligned;
    
    private float healthPercentage = 1f;

    public Vector2 currentWorldPosition;
    
    private static readonly int ProgressID = Shader.PropertyToID("_Progress");

    private void Start()
    {
        crossHair = GetComponent<RectTransform>();
        cursorImage = GetComponent<Image>();
        cursorMaterial = cursorImage.material;
        mainCamera = Camera.main;
        Cursor.visible = false;
        
        healthSlider.value = healthPercentage;
    }

    private void Update()
    {
        UpdateCrosshairPosition();
    }

    private void UpdateCrosshairPosition()
    {
        currentWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        crossHair.position = currentWorldPosition;
    }

    public void SetCrosshairRotation(float angle, float angleDifference)
    {
        crossHair.rotation = Quaternion.Euler(0, 0, angle);
        
        var t = Mathf.InverseLerp(0, 180, angleDifference);
        cursorImage.color = Color.Lerp(alignmentColor, nonAlignmentColor, t);
    }
        
    public void UpdateCrosshairProgress(float reloadProgress)
    {
        cursorMaterial.SetFloat(ProgressID, reloadProgress);
    }
    
    public void SetAmmoText(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo}/{maxAmmo}";
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthPercentage = currentHealth / maxHealth;
        healthSlider.value = healthPercentage;
    }
}