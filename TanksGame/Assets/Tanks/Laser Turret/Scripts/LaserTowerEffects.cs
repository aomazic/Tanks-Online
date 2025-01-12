using UnityEngine;

public class LaserTowerEffects : MonoBehaviour, ITowerEffects
{
    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip warmupSound;
    [SerializeField]
    private AudioClip fireSound;
    [SerializeField]
    private AudioClip rotationSound;
    [SerializeField]
    private AudioClip cooldownSound;

    private int isFiringHash;
    private AudioSource audioSource;
    private Animator animator;
    private LaserTower laserTower;
    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        laserTower = GetComponentInParent<LaserTower>();
        
        isFiringHash = Animator.StringToHash("IsFiring");
    }

    public void StartFiring()
    {
        animator.SetBool(isFiringHash, true);
        PlaySound(warmupSound, false);
    }

    public void StopFiring()
    {
        animator.SetBool(isFiringHash, false);
        PlaySound(cooldownSound, false);
    }
    
    public void StartRotation()
    {
        PlaySound(rotationSound, true);
    }
    
    public void StopRotation()
    {
        audioSource.Stop();
    }

    private void PlaySound(AudioClip clip, bool loop)
    {
        if (!audioSource || !clip)
        {
            return;
        }

        if (audioSource.isPlaying && audioSource.clip == clip)
        {
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = 0.5f; 
        audioSource.Play();
    }
    
    
    public void OnWarmupComplete()
    {
        PlaySound(fireSound, true);
        laserTower.StartContinuousFiring();
    }
}