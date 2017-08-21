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
        {
            _pathfinder.FindPath(endNode);
        }
            

        if(_pathfinder.path.Count > 0)
        {
            _controller.ignoreControllers.Clear();
        }

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

        _controller.UpdateVisibility();
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
            HandleNoPathToTargetControllerFound();
            return;
        }

        if (_controller.targetController == null)
        {
            _controller.MoveTo(_targetControllerPosition);
            return;
        }

        if (_controller.targetController.IntersectsPoint(nextTargetNode.gridPosPoint))
        {
            ReachedTarget();
        }

        // Didn't find path
        // Do this check last
        else if (_pathfinder.path.Count == 0)
        {
            HandleNoPathToTargetControllerFound();
        }
    }

    void ReachedTarget()
    {
        if (_targetController.controllerType == BaseController.CONTROLLER_TYPE.UNIT)
        {
            ReachedTargetUnit();
        }

        else if (_targetController.controllerType == BaseController.CONTROLLER_TYPE.BUILDING)
        {
            ReachedTargetBuilding();
        }

        else if (_targetController.controllerType == BaseController.CONTROLLER_TYPE.STATIC_RESOURCE)
        {
            ReachedTargetStaticResource();
        }

        else
        {
            _controller.TransitionToState(_controller.idleState);
        }

        _controller.FaceController(_targetController);
    }

    void ReachedTargetUnit()
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

    void ReachedTargetBuilding()
    {
        if (_targetController.playerID == PlayerManager.myPlayerID)
        {
            if (_controller._unitStats.builder)
            {
                _controller.TransitionToState(_controller.buildState);
            }
        }

        else
        {
            _controller.TransitionToState(_controller.attackState);
        }
    }

    void ReachedTargetStaticResource()
    {
        if (_controller._unitStats.gatherer)
        {
            _controller.TransitionToState(_controller.gatherState);
        }
    }

    void HandleNoPathToTargetControllerFound()
    {
        // Only seek resources close by if we are close to target resource
        if (_targetController.controllerType == BaseController.CONTROLLER_TYPE.STATIC_RESOURCE
            && Grid.instance.GetDistanceBetweenTiles(
                _controller._pathfinder.currentStandingOnNode.parentTile, 
                _targetController.GetPrimaryTile()) <= _controller._unitStats.visionRange * 10)
        {
            if (_controller._unitStats.gatherer)
            {
                _controller.ignoreControllers.Add(_targetController);
                _controller.SeekClosestResource(_targetController.title);
            }
        }

        else
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}