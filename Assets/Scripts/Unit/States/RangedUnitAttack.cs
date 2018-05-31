using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnitAttack : UnitAttack
{
    protected override void SetAttackSpeed()
    {
        attackSpeed = _controller._unitStats.attackSpeedRanged;
    }

    protected override bool ContinueToAttack()
    {
        // Is enemy still close enought?
        // Buildings never move
        // Use different logic to calculate distance to buildings
        if(_controller.targetController.controllerType == CONTROLLER_TYPE.BUILDING)
        {
            _controller.distanceToTarget = Grid.instance.GetDistanceBetweenNodes(
                _controller._pathfinder.currentStandingOnNode,
                _controller.targetNode);
        }

        else
        {
            _controller.distanceToTarget = Grid.instance.GetDistanceBetweenNodes(
                _controller._pathfinder.currentStandingOnNode,
                _controller.targetController.GetMiddleNode());
        }

        // Destroyed, dead or moved too far away
        return _controller.targetController != null
            && !_controller.targetController.dead
            && _controller.distanceToTarget <= (_controller._unitStats.range * 10);
    }

    protected override void PlayAnimation()
    {
        _controller.FaceController(_controller.targetController);
        _controller._animator.speed = 1.0f;
        _controller.timeSinceLastAttack = 0.0f;
        _controller._animator.Play("fire", -1, 0.0f);
    }
}
