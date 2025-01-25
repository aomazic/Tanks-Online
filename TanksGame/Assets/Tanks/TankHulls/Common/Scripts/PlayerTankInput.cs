using UnityEngine;

public class PlayerTankInput : MonoBehaviour, ITankInput
{
    private TankControls controls;

    public Vector2 MoveInput { get; private set; }
    public float RotateInput { get; private set; }
    public event System.Action OnFire;

    private void Awake()    
    {
        controls = new TankControls();
        controls.Tank.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Tank.Rotate.performed += ctx => RotateInput = ctx.ReadValue<float>();
        controls.Tank.Fire.performed += _ => OnFire?.Invoke();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}
