using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveToResourcePosition : UnitMoveToPosition
{
    public override void CheckTransitions()
    {
        // Reached target node
        if (nextTargetNode == endNode
            && Vector2.Distance(_transform.position, endNode.worldPosition) < 0.01f)
        {
            _controller.SeekClosestResource(_controller.resourceTitleCarrying);
        }

        // No path to follow
        if (endNode == null)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }

    protected override void HandleBeingBlockedFromPath()
    {
        _controller.SeekClosestResource(_controller.resourceTitleCarrying);
    }
}
