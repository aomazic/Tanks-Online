using System.Collections.Generic;
using UnityEngine;

public class ProjectileCannonEffects : TurretEffects
{
    [Header("Audio")]
    [SerializeField] protected List<AudioClip> fireSounds;
    
    private int fireHash;
    
    private void Start()
    {
        fireHash = Animator.StringToHash("Fire");
    }   
    
    public void Fire()
    {
        Animator.SetBool(fireHash, true);
        PlayFireSound(GetRandomFireSound(), false);
    }
    
    private void OnFireAnimationEnd()
    {
        Animator.SetBool(fireHash, false);
    }
    
    private AudioClip GetRandomFireSound()
    {
        if (fireSounds == null || fireSounds.Count == 0)
        {
            return null;
        }
        var randomIndex = Random.Range(0, fireSounds.Count);
        return fireSounds[randomIndex];
    }
}
