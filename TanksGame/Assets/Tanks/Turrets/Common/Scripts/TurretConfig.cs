using UnityEngine;

[CreateAssetMenu(fileName = "TurretConfig", menuName = "Tank/TurretConfig")]
public class TurretConfig : ScriptableObject
{
    [Header("Rotation")]
    public float rotationSpeed = 180f;
    public float rotationAcceleration = 5f;
    public float aimThreshold = 1f;

    [Header("Combat")]
    public float fireRate = 0.5f;
    public float projectileDamage = 20f;
    public float projectileSpeed = 15f;
    public int totalAmmo = 10;

    
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
    public float FireRate
    {
        get => fireRate;
        set => fireRate = value;
    }
    public float ProjectileDamage
    {
        get => projectileDamage;
        set => projectileDamage = value;
    }
    public float ProjectileSpeed
    {
        get => projectileSpeed;
        set => projectileSpeed = value;
    }
    public int TotalAmmo
    {
        get => totalAmmo;
        set => totalAmmo = value;
    }
}
