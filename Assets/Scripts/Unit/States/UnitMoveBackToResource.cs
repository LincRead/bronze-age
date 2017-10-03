using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveBackToResource : UnitMoveToController
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _controller.harvestingResource = true;

        // Remember so we can go back to continue harvesting after derlivering
        // resources to a delivery point, or find a similair resource if it gets depleted by the time we reach it.
        _controller.lastResourceGatheredPosition = _controller.targetController.GetPosition();
        _controller.resourceTitleCarrying = _controller.targetController.title;
    }

    // Make sure we move to resource pos if target Resource gets destroyed 
    // before we reach it.
    protected override void ReachedNextTargetNode()
    {
        if (_controller.targetController == null)
        {
            _controller.MoveToResourcePos(_targetControllerPosition);
            return;
        }

        base.ReachedNextTargetNode();
    }

    protected override void HandleBeingBlockedFromPath()
    {
        _controller.SeekClosestResource(_controller.resourceTitleCarrying);
    }
}
