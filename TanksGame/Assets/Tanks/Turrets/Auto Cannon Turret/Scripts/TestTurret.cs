using System.Collections;
using UnityEngine;

public class TestTurret : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("References")]
    [SerializeField] private Transform firePoint;
    
    
    private AutoCannonEffects towerEffects;

    private void Start()
    {
        towerEffects = GetComponent<AutoCannonEffects>();
        StartCoroutine(FireContinuously());
    }

    private IEnumerator FireContinuously()
    {
        while (true)
        {
            FireProjectile();
            yield return new WaitForSeconds(2f);
        }
    }

    private void FireProjectile()
    {
        var projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        var rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = firePoint.up * 5f; 
        towerEffects.Fire();
    }
}