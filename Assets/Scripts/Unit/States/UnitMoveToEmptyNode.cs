using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveToEmptyNode : UnitMoveTo
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        FindPathToTarget();
    }

    protected override void FindPathToTarget()
    {
        BaseController blockingController = _pathfinder.currentStandingOnNode.parentTile.controllerOccupying;
        
        endNode = Grid.instance.FindClosestWalkableNode(_controller.GetMiddleNode());

        if(blockingController != null)
        {
            // Make sure unit can use pathfinding to get away from blocking building
            Grid.instance.SetWalkableValueForTiles(blockingController, true);
        }

        _pathfinder.FindPath(endNode);

        if (blockingController != null 
            && !blockingController._basicStats.walkable
            && !blockingController.dead)
        {
            Grid.instance.SetWalkableValueForTiles(blockingController, false);
        }
    }

    public override void CheckTransitions()
    {
        // Reached target node
        // if (nextTargetNode == endNode && Vector2.Distance(_transform.position, endNode.worldPosition) <= 0.02f)
		if(nextTargetNode == endNode && _pathfinder.GetNodeFromPoint(_transform.position) == nextTargetNode)	
        {
            _controller.TransitionToState(_controller.lastState);
        }

        // No path to follow
        else if (endNode == null)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}
