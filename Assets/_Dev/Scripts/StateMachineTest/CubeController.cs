using UnityEngine;

public class CubeController : MonoBehaviour
{
    private StateMachine stateMachine;
    private RotateState rotateState;
    private IdleState idleState;
    private InputManager inputManager;
    
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();

        stateMachine = new StateMachine();

        rotateState = new RotateState(transform);
        idleState = new IdleState();

        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.Execute();

        if (inputManager.LeftClickPressedThisFrame)
        {
            HandleClick();
        }
    }

    public void HandleClick()
    {
        if (stateMachine.CurrentState == idleState)
            {
                stateMachine.TransitionTo(rotateState);
            }
            else if (stateMachine.CurrentState == rotateState)
            {
                stateMachine.TransitionTo(idleState);
            }
    }
}
