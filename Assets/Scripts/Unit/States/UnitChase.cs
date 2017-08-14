using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/chase")]
public class UnitChase : UnitMoveTo
{
    BaseController _targetController;
    Vector2 _targetControllerPosition;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _targetController = _controller.targetController;
        _targetControllerPosition = _targetController.GetPosition();

        FindPathToTarget();
        PlayRunAnimation();
    }

    protected override void FindPathToTarget()
    {
        // Find path
        Node node = _pathfinder.GetNodeFromPoint(_targetController.GetPrimaryNode().worldPosition);

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

        if (Grid.instance.GetDistanceBetweenNodes(
            _controller._pathfinder.currentStandingOnNode, _targetController.GetPrimaryNode()) <= 20)
        {
            _controller.TransitionToState(_controller.attackState);
        }
    }
}
