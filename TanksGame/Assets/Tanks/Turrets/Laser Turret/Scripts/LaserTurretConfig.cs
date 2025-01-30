using UnityEngine;

[CreateAssetMenu(fileName = "LaserTurretConfig", menuName = "Scriptable Weapons/Turrets//Laser Turret Config")]
public class LaserTurretConfig : ScriptableObject
{
    [Header("Laser Turret Properties")]
    public float energyDepletionRate; 
    public float energyRegenRate;
    public float energyCapacity;
    public float laserDamage;
    public float maxRange;
    
    public float EnergyDepletionRate
    {
        get => energyDepletionRate;
        set => energyDepletionRate = value;
    }
    
    public float EnergyRegenRate
    {
        get => energyRegenRate;
        set => energyRegenRate = value;
    }
    
    public float EnergyCapacity
    {
        get => energyCapacity;
        set => energyCapacity = value;
    }
    
    public float LaserDamage
    {
        get => laserDamage;
        set => laserDamage = value;
    }
}
