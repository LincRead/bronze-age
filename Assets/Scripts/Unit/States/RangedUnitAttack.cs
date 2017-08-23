using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnitAttack : UnitAttack
{
    protected override bool ContinueToAttack()
    {
        // Destroyed, dead or moved
        return _controller.distanceToTarget <= (_controller._unitStats.attackTriggerRadius * 10);
    }

    protected override void StopAttack()
    {
        if(_controller.targetController == null || _controller.targetController.dead)
        {
            _controller.TransitionToState(_controller.idleState);
        }

        else if(_controller.distanceToTarget <= _controller._unitStats.visionRange * 10)
        {
            _controller.MoveTo(_controller.targetController);
        }

        else
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}
