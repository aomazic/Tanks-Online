using System.Collections.Generic;
using UnityEngine;

public interface ITower: IDamagable
{
    float MaximumRange { get; set; }
    float MinimumRange { get; set; }
    float Accuracy { get; set; }
    float TurnSpeed { get; set; }
    Transform FirePoint { get; set; }
    IEnemy Target { get; set; }
    HashSet<IEnemy> enemiesInMaxRange { get; set; }
    ITowerEffects TowerEffects { get;}
    void setRanges();
    void NewEnemyDetected(IEnemy enemy);
    void EnemyOutOfRange(IEnemy enemy);
    void HandleTargetDeath(IEnemy enemy);
    void FindNewTarget();
    void RotateTurretTowardsTarget();
    bool IsTurretPointedAtTarget();
    void SetTarget(IEnemy target);
    void Fire();
}
