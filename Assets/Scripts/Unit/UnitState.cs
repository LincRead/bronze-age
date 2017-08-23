﻿using UnityEngine;
using System.Collections;

public class UnitState : ScriptableObject
{
    protected UnitStateController _controller;
    protected Node _targetStandingOnNode;
    protected float timeSinceStateChange = 0.0f;
    private bool playAnimation = false;

    public virtual void OnEnter(UnitStateController controller)
    {
        _controller = controller;
        playAnimation = true;

        if(_controller.targetController)
            _targetStandingOnNode = _controller.targetController.GetPrimaryNode();
    }

    protected virtual void PlayAnimation()
    {

    }

    public virtual void UpdateState()
    {
        if(playAnimation)
        {
            PlayAnimation();
            playAnimation = false;
        }

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