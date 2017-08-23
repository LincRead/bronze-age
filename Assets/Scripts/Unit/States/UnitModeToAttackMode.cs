using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackMode : UnitMoveToPosition
{
    protected override void ReachedNextTargetNode()
    {
        base.ReachedNextTargetNode();

        if(_controller.distanceToTarget <= _controller._unitStats.attackTriggerRadius * 10)
        {
            _controller.LookForNearbyEnemies();
        }
    }
}
