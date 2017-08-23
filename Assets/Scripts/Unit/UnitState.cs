using UnityEngine;
using System.Collections;

public class UnitState : ScriptableObject
{
    protected UnitStateController _controller;
    protected Node _targetStandingOnNode;
    protected float timeSinceStateChange = 0.0f;

    public virtual void OnEnter(UnitStateController controller)
    {
        _controller = controller;

        if(_controller.targetController)
            _targetStandingOnNode = _controller.targetController.GetPrimaryNode();
    }

    public virtual void UpdateState()
    {
        timeSinceStateChange += Time.deltaTime;

        DoActions();

        if(_controller.currentState == this)
        {
            CheckTransitions();
        }
    }

    public virtual void DoActions()
    {

    }

    public virtual void CheckTransitions()
    {

    }

    public virtual void OnExit()
    {

    }
}