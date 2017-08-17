using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "States/Unit states/move to position")]
public class UnitMoveToPosition : UnitMoveTo
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        FindPathToTarget();
        PlayRunAnimation();
    }

    protected override void FindPathToTarget()
    {
        endNode = _pathfinder.GetNodeFromPoint(_controller.targetPosition);

        if (endNode == null || !endNode.walkable)
            return;

        _pathfinder.FindPath(endNode);
    }

    public override void CheckTransitions()
    {
        // No path to follow
        if (endNode == null || timeSinceRouteBlocked >= timeBeforeGivingUpRoute)
        {
            _controller.TransitionToState(_controller.idleState);
            return;
        }
           
        // Reached target node
        if (nextTargetNode == endNode &&
            Vector2.Distance(_transform.position, endNode.worldPosition) < 0.01f)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}