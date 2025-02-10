using UnityEngine;
using UnityEngine.UI;

public class MainCrosshair : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private float maxDistanceFromTank = 5f;

    [Header("Visuals")]
    [SerializeField] private Color alignmentColor = Color.green;
    [SerializeField] private Color nonAlignmentColor = Color.red;

    private RectTransform crossHair;
    private Camera mainCamera;
    private Vector2 currentWorldPosition;
    private Image crossHairImage;
    private bool isAligned;

    public Vector2 WorldPosition => currentWorldPosition;

    private void Start()
    {
        crossHair = GetComponent<RectTransform>();
        crossHairImage = GetComponent<Image>();
        mainCamera = Camera.main;
        Cursor.visible = false;
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

    public void SetCrosshairRotation(float angle, bool shouldRotate)
    {
        // add logic for grading colors based on allignment
        crossHair.rotation = Quaternion.Euler(0, 0, angle);

        if (shouldRotate == !isAligned)
        {
            return;
        }

        if (shouldRotate)
        {
            OnNonAlligment();
        }
        else
        {
            OnAllignment();
        }
        isAligned = !shouldRotate;
    }

    private void OnAllignment()
    {
        crossHairImage.color = alignmentColor;
    }

    private void OnNonAlligment()
    {
        crossHairImage.color = nonAlignmentColor;
    }
}