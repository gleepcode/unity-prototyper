using UnityEngine;
public class RotateState : IState
{
    private Transform target;

    public RotateState(Transform target)
    {
        this.target = target;
    }

    public void Enter()
    {
        Debug.Log("Entered Rotate");
    }
    public void Execute()
    {
        target.Rotate(Vector3.up * 90f * Time.deltaTime);
    }
    public void Exit()
    {
        Debug.Log("Exited Rotate");
    }
}