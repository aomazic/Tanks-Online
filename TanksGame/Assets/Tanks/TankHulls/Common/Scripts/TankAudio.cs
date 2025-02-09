using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TankAudio : MonoBehaviour
{
    [Header("Engine Audio")]
    [SerializeField] private AudioSource engineIdleSource;
    [SerializeField] private AudioSource engineRunningSource;
    [SerializeField] private float engineCrossFadeTime = 0.5f;

    [Header("Track Audio")]
    [SerializeField] private AudioSource tracksSource;
    [SerializeField] private float trackSmoothTime = 0.1f;

    private float currentTrackVolume;
    private float trackVolumeVelocity;
    private float maxEngineVolume;

    public void Initialize(TankConfig config)
    {
        engineIdleSource.clip = config.engineIdleClip;
        engineRunningSource.clip = config.engineRunningClip;
        tracksSource.clip = config.trackLoopClip;
        
        maxEngineVolume = config.maxEngineVolume;

        engineIdleSource.loop = true;
        engineRunningSource.loop = true;
        tracksSource.loop = true;

        engineIdleSource.Play();
        engineRunningSource.Play();
        tracksSource.Play();
    }

    public void UpdateEngineAudio(bool isMoving, float speedPercent)
    {
        // Crossfade engine sounds
        var targetIdleVolume = isMoving ? maxEngineVolume * 0.25f : maxEngineVolume;
        var targetRunningVolume = isMoving ? maxEngineVolume : maxEngineVolume * 0.25f;

        engineIdleSource.volume = Mathf.Lerp(engineIdleSource.volume, targetIdleVolume, Time.deltaTime / engineCrossFadeTime);
        
        // Adjust running engine volume and pitch based on speed
        engineRunningSource.volume = Mathf.Lerp(engineRunningSource.volume, targetRunningVolume, Time.deltaTime / engineCrossFadeTime);
        engineRunningSource.volume *= Mathf.Lerp(maxEngineVolume * 0.3f, maxEngineVolume, speedPercent);
        engineRunningSource.pitch = Mathf.Lerp(0.8f, 1.2f, speedPercent);
    }

    public void UpdateTrackAudio(bool isMoving, float speedPercent)
    {
        // Smoothly adjust track volume based on movement
        var targetVolume = isMoving ? maxEngineVolume * 0.3f : 0f;
        currentTrackVolume = Mathf.SmoothDamp(currentTrackVolume, targetVolume, ref trackVolumeVelocity, trackSmoothTime);
        
        tracksSource.volume = currentTrackVolume;
        tracksSource.pitch = Mathf.Lerp(0.8f, 1.2f, speedPercent);
    }
}