using UnityEngine;
public class PlayerChaseState : IState
{
    private PlayerController player;

    public PlayerChaseState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("Player has entered Chase State");
    }
    public void Execute()
    {
        player.ChaseAttackTarget();

        if (!player.HasAttackTarget())
        {
            player.TransitionToIdleState();
        }
        if (player.IsInAttackRange())
        {
            player.TransitionToAttackState();
        }
    }
    public void Exit()
    {

    }
}