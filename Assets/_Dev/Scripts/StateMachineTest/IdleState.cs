using UnityEngine;
public class IdleState : IState
{
    public void Enter()
    {
        Debug.Log("Entered Idle");
    }
    public void Execute()
    {
        
    }
    public void Exit()
    {
        Debug.Log("Exited Idle");
    }
}