using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerInput : MonoBehaviour {
    [SerializeField] private Camera viewCamera;

    // variables to hold Action references
    InputAction pointAction;
    InputAction clickAction;

    NavMeshAgent agent;

    Vector3 moveTarget;
    bool hasMoveTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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

        if (hasMoveTarget)
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
                hasMoveTarget = true;
            }
        }
    }
    private void MovePlayer()
    {
        // Move the player to the target location
        agent.destination = moveTarget;

        if (agent.remainingDistance <= 1f)
        {
            hasMoveTarget = false;
        }

    }
}
