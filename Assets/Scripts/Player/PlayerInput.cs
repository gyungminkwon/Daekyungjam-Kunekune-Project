using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerInputAction Action { get; private set; }
    public float MoveInput => Action.Player.Move.ReadValue<float>();
    public bool IsSprint => Action.Player.Sprint.IsPressed();
    public bool IsJump => Action.Player.Jump.WasPressedThisFrame();
    public bool IsCrouch => Action.Player.Crouch.IsPressed();
    public bool IsInteract => Action.Player.Interact.WasPressedThisFrame();
    
    void Awake()
    {
        Action = new PlayerInputAction();
    }

    void OnEnable() => Action.Enable();
    void OnDisable() => Action.Disable();
}
