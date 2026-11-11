using UnityEngine;
using UnityEngine.InputSystem;

public class CubeInput : MonoBehaviour
{
    [SerializeField] private CubeController controller;
    private InputAction clickAction;

    private void Awake()
    {
        clickAction = InputSystem.actions.FindAction("Click");
    }

    private void Update()
    {
        if (clickAction.WasPressedThisFrame())
        {
            controller.HandleClick();
        }
    }
}