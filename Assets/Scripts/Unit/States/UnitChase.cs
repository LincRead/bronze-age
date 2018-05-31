using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/chase")]
public class UnitChase : UnitMoveTo
{
    UnitStateController _targetController;
    Vector2 _targetControllerPosition;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _targetController = (UnitStateController)_controller.targetController;
        _targetControllerPosition = _targetController.GetPosition();

        FindPathToTarget();
    }

    protected override void FindPathToTarget()
    {
        // Find path
        Node node = _pathfinder.GetNodeFromGridPos(
            (int)_targetController._pathfinder.currentStandingOnNode.gridPosPoint.x,
            (int)_targetController._pathfinder.currentStandingOnNode.gridPosPoint.y);

        if (node != null)
            _pathfinder.FindPath(node);
    }

    public override void CheckTransitions()
    {
        if (_pathfinder.path == null || _pathfinder.path.Count == 0)
        {
            _controller.TransitionToState(_controller.idleState);
            return;
        }

        if (_targetController == null)
        {
            _controller.MoveTo(_targetControllerPosition);
            return;
        }

        if (_controller._unitStats.canAttackRanged 
            && _controller.distanceToTarget <= _controller._unitStats.range * 10)
        {
            _controller.TransitionToState(_controller.rangedAttackState);
        }

        else if (_controller._unitStats.canAttackMelee 
            && Grid.instance.GetDistanceBetweenNodes(_controller._pathfinder.currentStandingOnNode, _targetController._pathfinder.currentStandingOnNode) <= 10)
        {
            _controller.TransitionToState(_controller.attackState);
        }
    }
}
