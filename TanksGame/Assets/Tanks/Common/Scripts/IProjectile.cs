using UnityEngine;

public interface IProjectile
{
    float Damage { get; set; }
    float ExplosionRadius { get; set; }
    float ExplosionForce { get; set; }
    float PenetrationRating { get; set; }
    float MaxRange { get; set; }
    GameObject ProjectilePrefab { get; set; }
    
    void Explode();
    void Penetrate();
    void DestroyProjectile();
}
