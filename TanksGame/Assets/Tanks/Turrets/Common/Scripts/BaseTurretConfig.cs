using UnityEngine;

[CreateAssetMenu(fileName = "BaseTurretConfig", menuName = "Weapons/Turrets/BaseTurretConfig")]
public class BaseTurretConfig : ScriptableObject
{
    [Header("Rotation")]
    public float rotationSpeed = 180f;
    public float rotationAcceleration = 5f;
    public float aimThreshold = 1f;
    
    public float RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }
    
    public float RotationAcceleration
    {
        get => rotationAcceleration;
        set => rotationAcceleration = value;
    }
    
    public float AimThreshold
    {
        get => aimThreshold;
        set => aimThreshold = value;
    }
}
