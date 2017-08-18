using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "States/Unit states/move to controller")]
public class UnitMoveToController : UnitMoveTo
{
    protected BaseController _targetController;
    protected Vector2 _targetControllerPosition;

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
        {
            Grid.instance.SetWalkableValueForTiles(_targetController, true);
        }

        // Find path
        endNode = _targetController.GetPrimaryNode();

        if (endNode != null)
            _pathfinder.FindPath(endNode);

        // Reset
        if (_targetController.controllerType != BaseController.CONTROLLER_TYPE.UNIT)
            Grid.instance.SetWalkableValueForTiles(_targetController.GetPosition(), _targetController.size, false);
    }

    protected override void ReachedNextTargetNode()
    {
        _pathfinder.path.Remove(nextTargetNode);

        // Did controller move?
        if (endNode != _controller.targetController.GetPrimaryNode())
            FindPathToTarget();

        // Fetch next target node
        if (_pathfinder.path.Count > 0)
        {
            nextTargetNode = _pathfinder.path[0];
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (_targetController != null)
        {
            _targetControllerPosition = _targetController.GetPosition();
        }
    }

    public override void CheckTransitions()
    {
        if (nextTargetNode == null)
        {
            _controller.TransitionToState(_controller.idleState);
            return;
        }

        if (_controller.targetController == null)
        {
            _controller.MoveTo(_targetControllerPosition);
            return;
        }

        if (_controller.targetController.IntersectsPoint(nextTargetNode.gridPosPoint))
        { 
            if (_targetController.controllerType == BaseController.CONTROLLER_TYPE.UNIT)
            {
                if (_targetController.playerID != _controller.playerID)
                {
                    _controller.TransitionToState(_controller.attackState);
                }

                else
                {
                    _controller.TransitionToState(_controller.idleState);
                }
            }

            else if (_targetController.controllerType == BaseController.CONTROLLER_TYPE.BUILDING)
            {
                if(_controller._unitStats.builder)
                {
                    _controller.TransitionToState(_controller.buildState);
                }
                
            }

            else if (_targetController.controllerType == BaseController.CONTROLLER_TYPE.STATIC_RESOURCE)
            {
                if(_controller._unitStats.gatherer)
                {
                    _controller.TransitionToState(_controller.gatherState);
                }
            }

            else
            {
                _controller.TransitionToState(_controller.idleState);
            }

            _controller.FaceController(_targetController);
        }

        // Didn't find path
        // Do this check last
        else if (_pathfinder.path.Count == 0)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}