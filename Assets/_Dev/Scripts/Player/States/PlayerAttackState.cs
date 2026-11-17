using UnityEngine;
public class PlayerAttackState : IState
{
    private PlayerController player;

    public PlayerAttackState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("Player has entered Attack State");
    }
    public void Execute()
    {
        if (!player.HasAttackTarget())
        {
            player.TransitionToIdleState();
            return;
        }
        if (!player.IsInAttackRange())
        {
            player.TransitionToChaseState();
            return;
        }
        else
        {
            player.Attack();
        }
    }
    public void Exit()
    {

    }
}