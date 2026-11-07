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
        UpdateRaycast(mousePosition);
    }
    private void UpdateRaycast(Vector2 mousePosition)
    {
        Ray ray = viewCamera.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);
        // if (clickAction.WasPressedThisFrame()){
        //     Debug.Log("Click!");
        // }

        // Add Physics.Raycast and hit handling here.
        
        
    }
}
