using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    [SerializeField] private Camera viewCamera;

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
        const float rayDistance = 100f;
        RaycastHit hit;

        Ray ray = viewCamera.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        if (clickAction.WasPressedThisFrame() && 
            Physics.Raycast(ray, out hit, rayDistance))
        {
            IClickable clickable = hit.collider.GetComponentInParent<IClickable>();
            clickable?.OnClicked();
        }
    }
}
