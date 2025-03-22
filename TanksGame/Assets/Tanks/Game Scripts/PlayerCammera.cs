using UnityEngine;

public class PlayerCammera : MonoBehaviour
{
[Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    [Header("Boundary Settings")]
    [SerializeField] private Collider2D levelBounds;
    
    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private float aspectRatio;
    private float orthographicSize;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        CacheCameraProperties();
    }

    private void CacheCameraProperties()
    {
        aspectRatio = mainCamera.aspect;
        orthographicSize = mainCamera.orthographicSize;
    }

    private void LateUpdate()
    {
        if (!target) return;
        
        var targetPosition = CalculateTargetPosition();
        var boundedPosition = ApplyBoundConstraints(targetPosition);
        ApplySmoothMovement(boundedPosition);
    }

    private Vector3 CalculateTargetPosition()
    {
        var basePosition = target.position + positionOffset;
        basePosition.z = transform.position.z; // Maintain camera depth
        return basePosition;
    }

    private Vector3 ApplyBoundConstraints(Vector3 targetPosition)
    {
        if (!levelBounds) return targetPosition;

        var bounds = levelBounds.bounds;
        var cameraExtents = CalculateCameraExtents();

        var clampedX = Mathf.Clamp(
            targetPosition.x,
            bounds.min.x + cameraExtents.x,
            bounds.max.x - cameraExtents.x
        );

        var clampedY = Mathf.Clamp(
            targetPosition.y,
            bounds.min.y + cameraExtents.y,
            bounds.max.y - cameraExtents.y
        );

        return new Vector3(clampedX, clampedY, targetPosition.z);
    }

    private Vector2 CalculateCameraExtents()
    {
        return new Vector2(
            orthographicSize * aspectRatio,
            orthographicSize
        );
    }

    private void ApplySmoothMovement(Vector3 targetPosition)
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!mainCamera) mainCamera = GetComponent<Camera>();
        CacheCameraProperties();
    }
    #endif
}
