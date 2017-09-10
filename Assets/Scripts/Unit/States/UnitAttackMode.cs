using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackMode : UnitMoveToPosition
{
    protected override void ReachedNextTargetNode()
    {
        base.ReachedNextTargetNode();

        _controller.LookForNearbyEnemyControllers();
    }
}
