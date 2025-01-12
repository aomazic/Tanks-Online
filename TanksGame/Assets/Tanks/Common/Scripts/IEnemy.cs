using UnityEngine;

public interface IEnemy : IDamagable
{
    IDamagable DamagableComponent { get; }
    event System.Action<IEnemy> OnDeath;
}
