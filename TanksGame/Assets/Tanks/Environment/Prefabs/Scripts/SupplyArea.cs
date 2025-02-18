using System.Collections;
using UnityEngine;

public class SupplyArea : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float refillRate = 1f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip refillSound;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var turretController = other.GetComponent<ProjectileTurretController>();
        if (turretController)
        {
            StartCoroutine(RefillProjectiles(turretController));
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var turretController = other.GetComponent<ProjectileTurretController>();
        if (turretController)
        {
            StopCoroutine(RefillProjectiles(turretController));
        }
    }

    private IEnumerator RefillProjectiles(ProjectileTurretController turretController)
    {
        while (turretController)
        {
            yield return new WaitForSeconds(refillRate);
            turretController.RefillProjectile();
        }
    }
}
