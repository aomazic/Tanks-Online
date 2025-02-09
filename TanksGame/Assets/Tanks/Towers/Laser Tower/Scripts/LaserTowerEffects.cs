using UnityEngine;

public class LaserTowerEffects : TurretEffects
{
    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip warmupSound;
    [SerializeField]
    private AudioClip cooldownSound;
    [SerializeField]
    protected AudioClip fireSound;
    
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
        PlayFireSound(warmupSound, false);
    }

    public void StopFiring()
    {
        Animator.SetBool(isFiringHash, false);
        PlayFireSound(cooldownSound, false);
    }
    
    public void OnWarmupComplete()
    {
        PlayFireSound(fireSound, true);
        laserTower.StartContinuousFiring();
    }
}