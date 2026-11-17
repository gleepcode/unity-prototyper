using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
    private PlayerStateMachine playerStateMachine;
    private PlayerIdleState playerIdleState;
    private PlayerMoveState playerMoveState;
    private PlayerChaseState playerChaseState;
    private PlayerAttackState playerAttackState;
    

    [SerializeField] private Camera viewCamera;
    [SerializeField] private InputManager inputManager;

    // Serialized fields for attack parameters
    [SerializeField] private float minDamage = 0.1f;
    [SerializeField] private float maxDamage = 1f;
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackSpeed = 0.4f;
    [SerializeField] private float lookAtSpeed = 100f;
    float nextAttackTime;

    NavMeshAgent agent;
    RaycastHit hit;

    // Variables to store the current move target and attack target
    Vector3 moveTarget;
    GameObject attackTarget;

    // Public properties to expose the move target and attack target
    public Vector3 MoveTarget => moveTarget;
    public GameObject AttackTarget => attackTarget;

    private void Awake()
    {
        // Creating instances of all concrete state classes
        // Pass PlayerController into each state
        playerIdleState = new PlayerIdleState(this);
        playerMoveState = new PlayerMoveState(this);
        playerChaseState = new PlayerChaseState(this);
        playerAttackState = new PlayerAttackState(this);

        playerStateMachine = new PlayerStateMachine();
        playerStateMachine.Initialize(playerIdleState);
        
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (inputManager.LeftClickPressedThisFrame)
        {
            Vector2 mousePosition = inputManager.MousePosition;
            UpdateRaycast(mousePosition);
        }

        playerStateMachine.Execute();
    }

    private void UpdateRaycast(Vector2 mousePosition)
    {   
        float rayDistance = 100f;
        Ray ray = viewCamera.ScreenPointToRay(mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.transform.CompareTag("Floor"))
            {
                HandleFloorClick();
            }
            else if (hit.transform.CompareTag("Attackable"))
            {
                HandleAttackableClick();
            }
            else return;
        }
    }

    private void HandleFloorClick()
    {
        TransitionToMoveState();
        ClearAttackTarget();
        agent.isStopped = false;
        moveTarget = hit.point;
    }

    private void HandleAttackableClick()
    {
        attackTarget = hit.transform.gameObject;
        nextAttackTime = 0f;
        TransitionToChaseState();
    }

    public void MoveToTarget()
    {
        LookAtTarget(moveTarget);
        // Move the player to the target location
        agent.destination = moveTarget;
    }

    public bool MoveTargetReached()
    {
        if (!agent.pathPending && !agent.hasPath) return true;
        else return false;
    }

    public void ChaseAttackTarget()
    {
        agent.isStopped = false;
        agent.destination = attackTarget.transform.position;
    }

    public void Attack()
    {
        agent.isStopped = true;

        if (Time.time < nextAttackTime)
        {
            return;
        }

        IDamageable damageable = attackTarget.GetComponentInParent<IDamageable>();
        damageable.OnDamage(DetermineAttackDamage());
        nextAttackTime = Time.time + attackSpeed;
    }

    public bool HasAttackTarget()
    {
        if (attackTarget == null)
        {
            return false;
        }
        else return true;
    }

    public bool IsInAttackRange()
    {
        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.transform.position);
        return distanceToTarget <= attackRange;
    }
    public void ClearAttackTarget()
    {
        attackTarget = null;
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

    private float DetermineAttackDamage()
    {
        float damage = Random.Range(minDamage, maxDamage);
        // Round to 1 decimal place
        return Mathf.Round(damage * 10.0f) * 0.1f;
    }

    
    // State transition methods for external access
    public void TransitionToIdleState()
    {
        playerStateMachine.TransitionTo(playerIdleState);
    }
    public void TransitionToMoveState()
    {
        playerStateMachine.TransitionTo(playerMoveState);
    }
   
    public void TransitionToChaseState()
    {
        playerStateMachine.TransitionTo(playerChaseState);
    }
    public void TransitionToAttackState()
    {
        playerStateMachine.TransitionTo(playerAttackState);
    }
}
