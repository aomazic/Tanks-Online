using UnityEngine;

public class AutoCannonEffects : TurretEffects
{
    
    private int fireHash;
    
    private void Start()
    {
        fireHash = Animator.StringToHash("Fire");
    }   
    
    public void Fire()
    {
        Animator.SetBool(fireHash, true);
        BaseAudioSource.Stop();
        PlaySound(fireSound, false);
    }
    
    private void OnFireAnimationEnd()
    {
        Animator.SetBool(fireHash, false);
    }
}
