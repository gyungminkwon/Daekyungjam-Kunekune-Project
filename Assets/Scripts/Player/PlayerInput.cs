using UnityEngine;

//기본값(0)보다 낮은 숫자를 주면, 게임 시작 시 다른 스크립트보다 Awake()가 무조건 먼저 실행됩니다
[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{
    public PlayerInputAction Action { get; private set; }
    public float MoveInput => Action.Player.Move.ReadValue<float>();
    public bool IsSprint => Action.Player.Sprint.IsPressed();
    public bool IsCrouch => Action.Player.Crouch.IsPressed();
    public bool IsInteract => Action.Player.Interact.WasPressedThisFrame();

    void Awake()
    {
        Action = new PlayerInputAction();
    }

    void OnEnable() => Action.Enable();
    void OnDisable() => Action.Disable();
}