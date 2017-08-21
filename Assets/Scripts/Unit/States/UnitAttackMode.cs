using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModeToAttackMode : UnitMoveToController
{
    protected override void ReachedNextTargetNode()
    {
        base.ReachedNextTargetNode();

        _controller.LookForNearbyEnemies();
    }
}
