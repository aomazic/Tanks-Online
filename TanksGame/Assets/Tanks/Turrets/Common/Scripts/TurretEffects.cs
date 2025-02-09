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
    protected AudioClip rotationSound;
    
    
    protected AudioSource BaseAudioSource => GetComponent<AudioSource>();
    protected Animator Animator => GetComponent<Animator>();
    
    private bool isRotating;
    
    
    public void UpdateRotationAudio(float angleDifference, float maxRotationSpeed,float rotationSpeed, bool shouldRotate)
    {
        if (!BaseAudioSource || !rotationSound)
        {
            return;
        }
        
        if(shouldRotate != isRotating)
        {
            isRotating = shouldRotate;
            if(isRotating)
            {
                BaseAudioSource.clip = rotationSound;
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
    
    protected void PlayFireSound(AudioClip clip, bool loop)
    {
        if (!fireAudioSource || !clip)
        {
            return;
        }

        if (fireAudioSource.isPlaying && fireAudioSource.clip == clip)
        {
            fireAudioSource.Stop();
        }

        fireAudioSource.clip = clip;
        fireAudioSource.loop = loop;
        fireAudioSource.Play();
    } 
}
