using UnityEngine;

public interface ITankInput
{
    Vector2 MoveInput { get; }
    float RotateInput { get; }
    event System.Action OnFire;
}
