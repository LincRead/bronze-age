using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnitMoveToNearbyEnemy : UnitMoveToNearbyEnemy
{
    public override void CheckTransitions()
    {
        if (_controller.targetController == null)
        {
            _controller.MoveTo(_targetControllerPosition);
            return;
        }

        if (_controller.distanceToTarget <= _controller._unitStats.range * 10)
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
