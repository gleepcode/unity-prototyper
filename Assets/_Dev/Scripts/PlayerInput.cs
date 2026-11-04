using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    [SerializeField] private Camera viewCamera;
    RaycastHit hit;

    // variables to hold Action references
    InputAction pointAction;
    InputAction clickAction;

    private void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        clickAction = InputSystem.actions.FindAction("Click");
    }
    private void Update()
    {
        Vector2 mousePosition = pointAction.ReadValue<Vector2>();

        if (clickAction.WasPressedThisFrame()){
            Debug.Log("Click!");
        }

        Ray ray = viewCamera.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);
    }
}
