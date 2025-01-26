using UnityEngine;

public class LaserTowerEffects : TurretEffects
{
    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip warmupSound;
    [SerializeField]
    private AudioClip cooldownSound;
    
    private int isFiringHash;
    
    private LaserTower laserTower;
    
    private void Start()
    {
        laserTower = GetComponentInParent<LaserTower>();
        isFiringHash = Animator.StringToHash("IsFiring");
    }

    public void StartFiring()
    {
        Animator.SetBool(isFiringHash, true);
        PlaySound(warmupSound, false);
    }

    public void StopFiring()
    {
        Animator.SetBool(isFiringHash, false);
        PlaySound(cooldownSound, false);
    }
    
    public void OnWarmupComplete()
    {
        PlaySound(fireSound, true);
        laserTower.StartContinuousFiring();
    }
}