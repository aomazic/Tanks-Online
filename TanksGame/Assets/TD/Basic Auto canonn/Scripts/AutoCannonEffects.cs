using UnityEngine;

public class AutoCannonEffects : MonoBehaviour, ITowerEffects
{
    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip fireSound;
    [SerializeField]
    private AudioClip rotationSound;
    
    private AudioSource audioSource;
    private Animator animator;
    
    private int fireHash;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        fireHash = Animator.StringToHash("Fire");
    }
    
    public void Fire()
    {
        animator.SetBool(fireHash, true);
        audioSource.Stop();
        PlaySound(fireSound, false);
    }
    
    private void OnFireAnimationEnd()
    {
        animator.SetBool(fireHash, false);
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
    
    public void StopRotation()
    {
        audioSource.Stop();
    }
    
    public void StartRotation()
    {
        PlaySound(rotationSound, true);
    }
}
