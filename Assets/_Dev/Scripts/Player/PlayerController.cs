using NUnit.Framework;
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

    // Flags to track if the player has a move target or an attack target
    private bool hasMoveTarget;
    private bool hasAttackTarget;

    // Public properties to expose the flags
    public bool HasMoveTarget => hasMoveTarget;
    public bool HasAttackTarget => hasAttackTarget;

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

        if (hasAttackTarget)
        {
            UpdateAttackTarget();
        }
        else if (hasMoveTarget)
        {
            Move();
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
            IClickable clickable = hit.collider.GetComponentInParent<IClickable>();
            clickable?.OnClicked();

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
        ClearAttackTarget();
        agent.isStopped = false;
        moveTarget = hit.point;
        hasMoveTarget = true;
    }

    private void HandleAttackableClick()
    {
        hasMoveTarget = false;
        attackTarget = hit.transform.gameObject;
        hasAttackTarget = true;
        nextAttackTime = 0f;
        MoveToAttackTarget();
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

        if (agent.pathPending)
        {
            return;
        }

        if (Vector3.Distance(transform.position, attackTarget.transform.position) > attackRange)
        {
            agent.isStopped = false;
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

        damageable.OnDamage(DetermineAttackDamage());
        nextAttackTime = Time.time + attackSpeed;
    }

    private void ClearAttackTarget()
    {
        attackTarget = null;
        hasAttackTarget = false;
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
    public void EnterIdleState()
    {
        playerStateMachine.TransitionTo(playerIdleState);
    }
    public void EnterMoveState()
    {
        playerStateMachine.TransitionTo(playerMoveState);
    }
   
    public void EnterChaseState()
    {
        playerStateMachine.TransitionTo(playerChaseState);
    }
    public void EnterAttackState()
    {
        playerStateMachine.TransitionTo(playerAttackState);
    }
}
