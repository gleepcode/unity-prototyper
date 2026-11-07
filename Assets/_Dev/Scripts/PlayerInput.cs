using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    [SerializeField] private Camera viewCamera;

    // variables to hold Action references
    InputAction pointAction;
    InputAction clickAction;

    private Vector3 moveTarget;

    private void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        clickAction = InputSystem.actions.FindAction("Click");
    }
    private void Update()
    {
        Vector2 mousePosition = pointAction.ReadValue<Vector2>();

        if (clickAction.WasPressedThisFrame())
        {
        UpdateRaycast(mousePosition);
        }

        if (moveTarget != null)
        {
            MovePlayer();
        }
    }
    private void UpdateRaycast(Vector2 mousePosition)
    {   
        const float rayDistance = 100f;
        RaycastHit hit;

        Ray ray = viewCamera.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            IClickable clickable = hit.collider.GetComponentInParent<IClickable>();
            clickable?.OnClicked();

            if(hit.transform.CompareTag("Floor"))
            {
                moveTarget = hit.point;
            }
        }
    }
    private void MovePlayer()
    {
        // Move the player to the target location
    }
}
