using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/move to nearby enemy")]
public class MoveToNearbyEnemy : UnitMoveToController
{
    protected override void FindPathToTarget()
    {
        if (_targetController == null)
            return;

        // Find path
        endNode = _targetController.GetPrimaryNode();

        _pathfinder.maxDistanceToTargetNode = _controller._unitStats.attackDetectionRange * 2;

        if (endNode != null)
            _pathfinder.FindPath(endNode);

        // Reset
        _pathfinder.maxDistanceToTargetNode = -1;
    }

    public override void CheckTransitions()
    {
        if (_controller.targetController == null)
        {
            _controller.MoveTo(_targetControllerPosition);
            return;
        }

        if (_controller.targetController.IntersectsPoint(nextTargetNode.gridPosPoint))
        {
            _controller.TransitionToState(_controller.attackState);
        }

        // Didn't find path
        // Do this check last
        else if (_pathfinder.path.Count == 0)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}
