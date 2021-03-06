﻿using UnityEngine;
using System.Collections;

public class UnitState : ScriptableObject
{
    protected UnitStateController _controller;
    protected Node _targetStandingOnNode;
    protected float timeSinceStateChange = 0.0f;
    protected bool playAnimationAtStart = false;

    public virtual void OnEnter(UnitStateController controller)
    {
        timeSinceStateChange = 0.0f;
        _controller = controller;
        playAnimationAtStart = true;
    }

    protected virtual void PlayAnimation()
    {

    }

    public virtual void UpdateState()
    {
        if (playAnimationAtStart)
        {
            PlayAnimation();
            playAnimationAtStart = false;
        }

        timeSinceStateChange += Time.deltaTime;

        if (_controller.currentState == this)
        {
            DoActions();
        }

        if (_controller.currentState == this)
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