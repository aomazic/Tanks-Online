using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SupplyArea : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float refillRate = 1f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip refillLoopSound;
    [SerializeField] private AudioClip refillEndSound;
    [SerializeField] private AudioClip refillBeginSound;
    
    private AudioSource audioSource;
    private Coroutine currentRefillCoroutine;
    private bool isRefilling;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = refillLoopSound;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other) return;
        
        var turretController = other.GetComponent<ProjectileTurretController>();
        if (!turretController) return;

        var tankController = other.GetComponent<TankController>();
        if (tankController)
        {
            tankController.OnDestroyed += StopRefilling;
        }

        PlayOneShot(refillBeginSound);
        StartRefilling(turretController);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other) return;
        
        var turretController = other.GetComponent<ProjectileTurretController>();
        if (!turretController) return;

        var tankController = turretController.tankHullController;
        if (tankController)
        {
            tankController.OnDestroyed -= StopRefilling;
        }
        
        StopRefilling(tankController);
        PlayOneShot(refillEndSound);
    }

    private void StartRefilling(ProjectileTurretController turretController)
    {
        isRefilling = true;
        audioSource.Play();
        currentRefillCoroutine = StartCoroutine(RefillProjectiles(turretController));
    }

    private void StopRefilling(IDamagable _)
    {
        if (!isRefilling) return;
        
        if (currentRefillCoroutine != null)
        {
            StopCoroutine(currentRefillCoroutine);
            currentRefillCoroutine = null;
        }

        isRefilling = false;
        audioSource.Stop();
    }

    private IEnumerator RefillProjectiles(ProjectileTurretController turretController)
    {
        if (!turretController) yield break;

        var waitTime = new WaitForSeconds(refillRate);
        
        while (turretController.enabled)
        {
            yield return waitTime;
            turretController.RefillProjectile();
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (!clip || !audioSource) return;
        audioSource.PlayOneShot(clip);
    }
}