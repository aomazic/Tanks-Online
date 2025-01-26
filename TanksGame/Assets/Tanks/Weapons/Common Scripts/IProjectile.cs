using UnityEngine;

public interface IProjectile
{
    GameObject ProjectilePrefab { get; }
    ProjectileStats Stats { get; }
    
    void Explode();
    void Penetrate();
    void DestroyProjectile();
}
