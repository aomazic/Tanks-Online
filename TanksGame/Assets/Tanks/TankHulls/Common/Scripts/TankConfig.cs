using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TankConfig", menuName = "Tank/Configuration")]
public class TankConfig : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 150f;
    public float acceleration = 5f;
    public float deceleration = 8f;
    public float reverseSpeedMultiplier = 0.7f;

    [Header("Audio")]
    public AudioClip engineIdleClip;
    public AudioClip engineRunningClip;
    public AudioClip trackLoopClip;
    public List<AudioClip> destructionAudioClips;
    [Range(0, 1)] public float maxEngineVolume = 0.8f;
    
    [Header("Stats")]
    public float maxHealth = 100f;
    public float armorRating = 1f;
    
    
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    
    public float RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }
    
    public float Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }
    
    public float Deceleration
    {
        get => deceleration;
        set => deceleration = value;
    }
    
    public float ReverseSpeedMultiplier
    {
        get => reverseSpeedMultiplier;
        set => reverseSpeedMultiplier = value;
    }
    
    public AudioClip EngineIdleClip
    {
        get => engineIdleClip;
        set => engineIdleClip = value;
    }
    
    public AudioClip EngineRunningClip
    {
        get => engineRunningClip;
        set => engineRunningClip = value;
    }
    
    public AudioClip TrackLoopClip
    {
        get => trackLoopClip;
        set => trackLoopClip = value;
    }
    
    public List<AudioClip> DestructionAudioClips
    {
        get => destructionAudioClips;
        set => destructionAudioClips = value;
    }
    
    public float MaxEngineVolume
    {
        get => maxEngineVolume;
        set => maxEngineVolume = value;
    }
    
    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    
    public float ArmorRating
    {
        get => armorRating;
        set => armorRating = value;
    }
}