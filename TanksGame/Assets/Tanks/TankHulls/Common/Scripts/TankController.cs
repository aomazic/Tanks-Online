using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class TankController : MonoBehaviour, IDamagable
{
    [Header("Configuration")]
    [SerializeField] private TankConfig config;
    
    [Header("Audio")]
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioSource tracksAudioSource;
    
    [Header("Input")]
    [SerializeField] private InputActionAsset controls;
    
    public event Action<IDamagable> OnDestroyed;
    public event Action<IDamagable> OnDamaged; 
    
    public Rigidbody2D Rigidbody => rb;
    public Collider2D Collider => col;
    public float Health
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    public Transform Transform => transform;

    private Rigidbody2D rb;
    private Collider2D col;
    private InputAction moveAction;
    private InputAction rotateAction;
    private float currentSpeed;
    private float targetSpeed;
    private float currentRotation;
    private bool isMoving;
    private float currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        
        // Set up input
        var actionMap = controls.FindActionMap("Tank");
        moveAction = actionMap.FindAction("Move");
        rotateAction = actionMap.FindAction("Rotate");
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        HandleInput();
 //       UpdateAudio();
    }

    private void HandleInput()
    {
        // Read new input system values
        var moveInput = moveAction.ReadValue<Vector2>();
        var rotateInput = rotateAction.ReadValue<float>();

        // Tank movement calculations
        targetSpeed = moveInput.y * config.MoveSpeed;
        currentRotation = rotateInput * config.RotationSpeed;
        isMoving = !Mathf.Approximately(moveInput.sqrMagnitude, 0) || 
                 !Mathf.Approximately(rotateInput, 0);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyRotation();
    }

    // IDamagable implementation
    public void TakeDamage(float damage)
    {
        currentHealth -= damage * config.ArmorRating;
        OnDamaged?.Invoke(this);
        
        if(currentHealth <= 0) 
        {
            OnDestroyed?.Invoke(this);
            Die();
        }
    }

    public void Die()
    {
        // Play destruction effects
        // Handle game over logic
        gameObject.SetActive(false);
    }
    
    private void ApplyMovement()
    {
        var accelerationRate = (Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed)) ? 
            config.acceleration : config.deceleration;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelerationRate * Time.fixedDeltaTime);
        
        Vector2 movement = transform.up * currentSpeed;
        rb.linearVelocity = movement;
    }

    private void ApplyRotation()
    {
        var rotation = currentRotation * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);
    }

    private void UpdateAudio()
    {
        // Engine sound
        if (isMoving && engineAudioSource.clip != config.engineRunningClip)
        {
            engineAudioSource.clip = config.engineRunningClip;
            engineAudioSource.Play();
        }
        else if (!isMoving && engineAudioSource.clip != config.engineIdleClip)
        {
            engineAudioSource.clip = config.engineIdleClip;
            engineAudioSource.Play();
        }

        // Engine pitch based on speed
        var speedPercent = Mathf.Abs(currentSpeed) / config.moveSpeed;
        engineAudioSource.pitch = Mathf.Lerp(0.8f, 1.2f, speedPercent);
        engineAudioSource.volume = Mathf.Lerp(0.3f, config.maxEngineVolume, speedPercent);

        // Track sounds
        if (isMoving && !tracksAudioSource.isPlaying)
        {
            tracksAudioSource.clip = config.trackLoopClip;
            tracksAudioSource.Play();
        }
        else if (!isMoving && tracksAudioSource.isPlaying)
        {
            tracksAudioSource.Stop();
        }
    }
}