using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.TextCore;

public class PlayerInput : MonoBehaviour {
    [SerializeField] private Camera viewCamera;
    [SerializeField] private float minDamage = 0.1f;
    [SerializeField] private float maxDamage = 1f;
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackSpeed = 0.4f;
    [SerializeField] private float lookAtSpeed = 100f;

    // variables to hold Action references
    InputAction pointAction;
    InputAction clickAction;

    NavMeshAgent agent;
    RaycastHit hit;
    Vector3 moveTarget;
    GameObject attackTarget;
    bool hasMoveTarget;
    bool hasAttackTarget;
    float nextAttackTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

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

        if (hasAttackTarget)
        {
            UpdateAttackTarget();
        }
        else if (hasMoveTarget)
        {
            Move();
        }
    }
    private void UpdateRaycast(Vector2 mousePosition)
    {   
        const float rayDistance = 100f;

        Ray ray = viewCamera.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            IClickable clickable = hit.collider.GetComponentInParent<IClickable>();
            clickable?.OnClicked();

            if (hit.transform.CompareTag("Floor"))
            {
                ClearAttackTarget();
                agent.isStopped = false;
                moveTarget = hit.point;
                hasMoveTarget = true;
            }
            else if (hit.transform.CompareTag("Attackable"))
            {
                hasMoveTarget = false;
                attackTarget = hit.transform.gameObject;
                hasAttackTarget = true;
                nextAttackTime = 0f;
                MoveToAttackTarget();
            }
        }
    }
    private void Move()
    {
        LookAtTarget(moveTarget);
        // Move the player to the target location
        agent.destination = moveTarget;

        if (agent.remainingDistance <= 1f)
        {
            hasMoveTarget = false;
        }

    }

    private void MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.destination = attackTarget.transform.position;
    }

    private void UpdateAttackTarget()
    {
        // Check if attack target still exists
        if (attackTarget == null)
        {
            ClearAttackTarget();
            agent.ResetPath();
            agent.isStopped = false;
            return;
        }

        LookAtTarget(attackTarget.transform.position);

        // If the player issues a new command, or if the attackRange is exceded, stop attacking
        if (agent.pathPending || agent.remainingDistance > attackRange)
        {
            return;
        }
        agent.isStopped = true;

        if (Time.time < nextAttackTime)
        {
            return;
        }

        IDamageable damageable = attackTarget.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            Debug.LogWarning($"Attack target '{attackTarget.name}' does not implement IDamageable.", attackTarget);
            ClearAttackTarget();
            agent.isStopped = false;
            return;
        }

        damageable.OnDamage(RandomizeAttackDamage());
        nextAttackTime = Time.time + attackSpeed;
    }

    private void ClearAttackTarget()
    {
        attackTarget = null;
        hasAttackTarget = false;
    }

    private float RandomizeAttackDamage()
    {
        return Random.Range(minDamage, maxDamage);
    }

    private void LookAtTarget(Vector3 target)
    {
        // Calculate direction to target, ignoring the height (Y axis) to prevent tipping over
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            // Create the target rotation and smoothly interpolate (Slerp) toward it
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookAtSpeed);
        }
}
}
