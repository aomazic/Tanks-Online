using UnityEngine;

public class TurretEffects : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource fireAudioSource;
    [SerializeField]
    [Range(0, 1)] private float maxRotationVolume = 0.6f;
    
    [Header("Sound Effects")]
    [SerializeField]
    protected AudioClip fireSound;
    
    
    protected AudioSource BaseAudioSource => GetComponent<AudioSource>();
    protected Animator Animator => GetComponent<Animator>();
    
    private bool isRotating;
    
    
    public void UpdateRotationAudio(float angleDifference, float maxRotationSpeed,float rotationSpeed, bool shouldRotate)
    {
        if(shouldRotate != isRotating)
        {
            isRotating = shouldRotate;
            if(isRotating)
            {
                BaseAudioSource.Play();
            }
            else
            {
                BaseAudioSource.Stop();
            }
        }
        
        
        var volume = Mathf.Clamp01(angleDifference / 45f) * maxRotationVolume;
        var pitch = Mathf.Lerp(0.8f, 1.2f, rotationSpeed / maxRotationSpeed);

        BaseAudioSource.volume = volume;
        BaseAudioSource.pitch = pitch;
    }
    
    protected void PlaySound(AudioClip clip, bool loop)
    {
        if (!BaseAudioSource || !clip)
        {
            return;
        }

        if (BaseAudioSource.isPlaying && BaseAudioSource.clip == clip)
        {
            return;
        }

        BaseAudioSource.clip = clip;
        BaseAudioSource.loop = loop;
        BaseAudioSource.volume = 0.5f; 
        BaseAudioSource.Play();
    } 
}
