using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnitMoveToNearbyEnemy : UnitMoveToNearbyEnemy
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        if (_controller.distanceToTarget <= _controller._unitStats.range * 10)
        {
            _controller.TransitionToState(_controller.attackState);
        }
    }

    public override void CheckTransitions()
    {
        if (nextTargetNode == null)
        {
            HandleNoPathToTargetControllerFound();
        }

        // Didn't find path
        // Do this check last
        else if (_pathfinder.path.Count == 0)
        {
            HandleNoPathToTargetControllerFound();
        }

        else if (_controller.distanceToTarget <= _controller._unitStats.range * 10)
        {
            _controller.TransitionToState(_controller.attackState);
        }
    }
}
