using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnitAttack : UnitAttack
{
    protected override bool ContinueToAttack()
    {
        // Is enemy still close enught?
        // Buildings never move
        // Use different logic to calculate distance to buildings
        if(_controller.targetController.controllerType == CONTROLLER_TYPE.BUILDING)
        {
            _controller.distanceToTarget = Grid.instance.GetDistanceBetweenNodes(
                _controller._pathfinder.currentStandingOnNode,
                _controller.targetNode);
        }

        // Destroyed, dead or moved too far away
        return _controller.targetController != null
            && !_controller.targetController.dead
            && _controller.distanceToTarget <= (_controller._unitStats.range * 10);
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
