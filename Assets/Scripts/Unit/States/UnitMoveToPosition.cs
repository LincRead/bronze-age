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
    }

    protected override void FindPathToTarget()
    {
         endNode = Grid.instance.GetNodeFromWorldPoint(_controller.targetPosition);

        if (!endNode.walkable)
        {
            endNode = Grid.instance.FindClosestWalkableNode(Grid.instance.GetNodeFromWorldPoint(_controller.targetPosition));
        }

        _pathfinder.FindPath(endNode);
    }

    public override void CheckTransitions()
    {  
        // Reached target node
		if(nextTargetNode == endNode && _pathfinder.GetNodeFromPoint(_transform.position) == nextTargetNode)	
        {
            _controller.TransitionToState(_controller.idleState);
        }

        // No path to follow
        if (endNode == null)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    } 
}