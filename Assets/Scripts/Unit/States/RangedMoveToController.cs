using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMoveToController : UnitMoveToController
{
    protected override void ReachedNextTargetNode()
    {
        _controller.UpdateVisibility();

        if (Grid.instance.GetDistanceBetweenTiles(
                _controller._pathfinder.currentStandingOnNode.parentTile, _targetController.GetPrimaryTile()) <= _controller._unitStats.range * 10)
        {
            _controller.TransitionToState(_controller.attackState);
        }

        // Continue moving towards target
        else
        {
            _pathfinder.path.Remove(nextTargetNode);

            // Did controller move?
            if (endNode != _controller.targetController.GetPrimaryNode())
            {
                FindPathToTarget();
            }

            // Fetch next target node
            if (_pathfinder.path.Count > 0)
            {
                nextTargetNode = _pathfinder.path[0];
            }
        }
    }
}
