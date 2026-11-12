using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    InputAction pointAction;
    InputAction clickAction;
    InputAction attackMoveAction;

    public Vector2 MousePosition { get; private set; }
    public bool LeftClickPressedThisFrame { get; private set; }
    public bool AttackMovePressedThisFrame { get; private set; }

    // Implement later
    // public bool RightClickPressedThisFrame { get; private set; }
    
    void Awake()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        clickAction = InputSystem.actions.FindAction("Click");
        attackMoveAction = InputSystem.actions.FindAction("AttackMove");
    }

    void Update()
    {
        MousePosition = pointAction.ReadValue<Vector2>();

        LeftClickPressedThisFrame = clickAction.WasPressedThisFrame();

        AttackMovePressedThisFrame = attackMoveAction.WasPressedThisFrame();
    }
}
