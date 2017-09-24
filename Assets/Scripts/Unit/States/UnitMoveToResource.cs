﻿using System.Collections;
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

        else if(timeSinceRouteBlocked >= timeBeforeGivingUpRoute)
        {
            _controller.SeekClosestResource(_controller.resourceTitleCarrying);
        }

        // No path to follow
        else if (endNode == null)
        {
            _controller.TransitionToState(_controller.idleState);
        }

        // Didn't find path
        // Do this check last
        else if (_pathfinder.path.Count == 0 && endNode != _pathfinder.currentStandingOnNode)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}