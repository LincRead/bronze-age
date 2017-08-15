using UnityEngine;
using System.Collections;

public class UnitState : ScriptableObject
{
    protected UnitStateController _controller;
    protected float timeSinceStateChange = 0.0f;

    public virtual void OnEnter(UnitStateController controller)
    {
        _controller = controller;
    }

    public virtual void UpdateState()
    {
        timeSinceStateChange += Time.deltaTime;
        DoActions();
        CheckTransitions();
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