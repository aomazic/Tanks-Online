using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class TankController : MonoBehaviour, IDamagable
{
    [Header("Configuration")]
    [SerializeField] private TankConfig tankConfig;
    
    [Header("Input")]
    [SerializeField] private InputActionAsset controls;
    
    public event Action<IDamagable> OnDestroyed;
    public event Action<IDamagable> OnDamaged; 
    public Rigidbody2D Rigidbody => rb;
    public Collider2D Collider => col;
    public float Health { get;  set; }
    public Transform Transform => transform;

    private Rigidbody2D rb;
    private Collider2D col;
    private InputAction moveAction;
    private InputAction rotateAction;
    private bool isMoving;
    private float currentSpeed;
    private float targetSpeed;
    private float currentRotation;
    private float currentHealth;
    private TankAudio tankAudio;

    
    void Start()
    {
        tankAudio = GetComponent<TankAudio>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        
        // Set up input
        var actionMap = controls.FindActionMap("Tank");
        moveAction = actionMap.FindAction("Move");
        rotateAction = actionMap.FindAction("Rotate");
        
        
        currentHealth = tankConfig.MaxHealth;
        Health = currentHealth;
        
        tankAudio.Initialize(tankConfig);
    }
    
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Read new input system values
        var moveInput = moveAction.ReadValue<Vector2>();
        var rotateInput = rotateAction.ReadValue<float>();

        // Tank movement calculations
        targetSpeed = moveInput.y * tankConfig.MoveSpeed;
        currentRotation = rotateInput * tankConfig.RotationSpeed;
        
        isMoving = moveInput.y != 0 || rotateInput != 0;
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyRotation();
        HandleAudio();
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage / tankConfig.ArmorRating;
        OnDamaged?.Invoke(this);
        
        if(currentHealth <= 0) 
        {
            OnDestroyed?.Invoke(this);
            Die();
        }
    }
    
    private void HandleAudio()
    {
        // Calculate audio parameters
        var speedPercent = Mathf.Abs(currentSpeed) / tankConfig.moveSpeed;
        
        // Update audio
        tankAudio.UpdateEngineAudio(isMoving, speedPercent);
        tankAudio.UpdateTrackAudio(isMoving, speedPercent);
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
            tankConfig.acceleration : tankConfig.deceleration;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelerationRate * Time.fixedDeltaTime);
        
        Vector2 movement = transform.up * currentSpeed;
        rb.linearVelocity = movement;
    }

    private void ApplyRotation()
    {
        var rotation = currentRotation * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);
    }
}