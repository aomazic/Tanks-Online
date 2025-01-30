using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileTurretConfig", menuName = "Weapons/Turrets/Projectile Turret Config")]
public class ProjectileTurretConfig : ScriptableObject
{
    [Header("Combat")]
    public float fireRate = 0.5f;
    public float projectileDamage = 20f;
    public float projectileSpeed = 15f;
    public int totalAmmo = 10;
    
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
