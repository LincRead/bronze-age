using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/move to nearby enemy")]
public class UnitMoveToNearbyEnemy : UnitMoveToController
{
    protected override void FindPathToTarget()
    {
        _pathfinder.maxDistanceToTargetNode = _controller._unitStats.attackTriggerRadius * 10;

        base.FindPathToTarget();

        // Reset
        _pathfinder.maxDistanceToTargetNode = -1;
    }
}