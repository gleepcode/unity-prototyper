using UnityEngine;
public class PlayerIdleState : IState
{
    private PlayerController player;

    public PlayerIdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("Player has entered Idle State");
    }

    public void Execute()
    {
        
    }
    
    public void Exit()
    {

    }
}