using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "States/Unit states/move to controller")]
public class UnitMoveToController : UnitMoveTo
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
        if (_targetController == null)
            return;

        // Make sure unit can use pathfinding to controller
        if(_targetController.controllerType != BaseController.CONTROLLER_TYPE.UNIT)
            Grid.instance.SetWalkableValueForTiles(_targetController, true);

        // Find path
        Node node = _pathfinder.GetNodeFromPoint(_targetController.GetPrimaryNode().worldPosition);

        if (node != null)
            _pathfinder.FindPath(node);

        // Reset
        if (_targetController.controllerType != BaseController.CONTROLLER_TYPE.UNIT)
            Grid.instance.SetWalkableValueForTiles(_targetController.GetPosition(), _targetController.size, false);
    }

    public override void CheckTransitions()
    {
        if (_pathfinder.path == null || _pathfinder.path.Count == 0)
        {
            _controller.TransitionToState(_controller.idleState);
            return;
        }

        if(_targetController == null)
        {
            _controller.MoveTo(_targetControllerPosition);
            return;
        }

        if (_controller.targetController.IntersectsPoint(nextTargetNode.gridPosPoint))
        { 
            if (_controller._unitStats.builder && _targetController.controllerType == BaseController.CONTROLLER_TYPE.BUILDING)
            {
                _controller.TransitionToState(_controller.buildState);
            }

            else if (_controller._unitStats.gatherer && _targetController.controllerType == BaseController.CONTROLLER_TYPE.STATIC_RESOURCE)
            {
                _controller.TransitionToState(_controller.gatherState);
            }

            else if (_targetController.controllerType == BaseController.CONTROLLER_TYPE.UNIT)
            {
                _controller.TransitionToState(_controller.attackState);
            }

            else
            {
                _controller.TransitionToState(_controller.idleState);
            }
        }
    }
}