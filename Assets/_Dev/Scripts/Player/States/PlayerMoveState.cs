using UnityEngine;
public class PlayerMoveState : IState
{
    private PlayerController player;

    public PlayerMoveState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("Player has entered Move State");
    }
    public void Execute()
    {   
        player.MoveToTarget();

        if (player.MoveTargetReached())
        {
            player.TransitionToIdleState();
        }
    }
    public void Exit()
    {

    }
}