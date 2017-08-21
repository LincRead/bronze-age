using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackMode : UnitMoveToPosition
{
    protected override void ReachedNextTargetNode()
    {
        base.ReachedNextTargetNode();

        if(Grid.instance.GetDistanceBetweenTiles(
            _controller._pathfinder.currentStandingOnNode.parentTile, 
            endNode.parentTile) <= _controller._unitStats.visionRange * 10)
        _controller.LookForNearbyEnemies();
    }
}
