using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "States/Unit states/move to position")]
public class UnitMoveToPosition : UnitMoveTo
{
    Node targetNode;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        FindPathToTarget();
        PlayRunAnimation();
    }

    protected override void FindPathToTarget()
    {
        targetNode = _pathfinder.GetNodeFromPoint(_controller.targetPosition);

        if (targetNode == null)
            return;

        if (!targetNode.walkable)
            return;

        _pathfinder.FindPath(targetNode);
    }

    public override void CheckTransitions()
    {
        // No path to follow
        if (_pathfinder.path == null || targetNode == null)
            _controller.TransitionToState(_controller.idleState);

        // Reached target node
        else if (Vector2.Distance(_transform.position, targetNode.worldPosition) < 0.01f)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}