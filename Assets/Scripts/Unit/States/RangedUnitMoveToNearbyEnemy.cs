using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnitMoveToNearbyEnemy : UnitMoveToNearbyEnemy
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        if (WitinRange())
        {
            _controller.TransitionToState(_controller.rangedAttackState);
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

        else if (WitinRange())
        {
            _controller.TransitionToState(_controller.rangedAttackState);
        }
    }

    bool WitinRange()
    {
        return _controller.distanceToTarget <= _controller._unitStats.range * 10;
    }

    protected override bool ShouldGetNextTargetNode()
    {
        return _controller.distanceToTarget > _controller._unitStats.range * 10;
    }
}
