using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileStats", menuName = "Weapons/ProjectileStats")]
public class ProjectileStats : ScriptableObject
{
    [Header("Combat Properties")]
    [SerializeField, Range(1, 100)] private float damage = 10f;
    [SerializeField, Min(0)] private float explosionRadius = 5f;

    [Header("Physics")]
    [SerializeField] private float explosionForce = 100f;
    [SerializeField, Tooltip("How many surfaces can penetrate")] 
    private float penetrationRating = 0f;

    [Header("Behavior")]
    [SerializeField] private float maxRange = 20f;

    public float Damage
    {
        get => damage;
        set => damage = value;
    }

    public float ExplosionRadius
    {
        get => explosionRadius;
        set => explosionRadius = value;
    }

    public float ExplosionForce
    {
        get => explosionForce;
        set => explosionForce = value;
    }

    public float PenetrationRating
    {
        get => penetrationRating;
        set => penetrationRating = value;
    }

    public float MaxRange
    {
        get => maxRange;
        set => maxRange = value;
    }
}