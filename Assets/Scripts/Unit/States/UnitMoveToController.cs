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

        if(_pathfinder.path.Count > 0)
        {
            // Fetch this for initial intersect target node check
            nextTargetNode = _pathfinder.path[0];

            // Already intersecting target?
            IntersectingTarget();
        }
    }

    protected override void FindPathToTarget()
    {
        if (_controller.targetController == null)
            return;

        // Make sure unit can use pathfinding to controller
        if(_targetController.controllerType != CONTROLLER_TYPE.UNIT)
        {
            Grid.instance.SetWalkableValueForTiles(_targetController, true);
        }

        endNode = _targetController.GetMiddleNode();

        if (endNode != null)
        {
            _pathfinder.FindPath(endNode);
        }

        // Reset
        if (_targetController.controllerType != CONTROLLER_TYPE.UNIT)
        {
            Grid.instance.SetWalkableValueForTiles(_targetController.GetPosition(), _targetController.size, false);
        }

        // Found path
        if (_pathfinder.path.Count > 0)
        {
            _controller.ignoreControllers.Clear();

            // Ignore unwalkable nodes
            while (_pathfinder.path.Count > 0 && !_pathfinder.path[_pathfinder.path.Count - 1].walkable)
            {
                _pathfinder.path.Remove(_pathfinder.path[_pathfinder.path.Count - 1]);
            }

            // endNode is last node in path
            endNode = _controller._pathfinder.path[_controller._pathfinder.path.Count - 1];

            // Set distance based on first node to end node
            _controller.distanceToTarget = Grid.instance.GetDistanceBetweenNodes(
                _controller._pathfinder.currentStandingOnNode,
                endNode);
        }
    }

    protected override void ReachedNextTargetNode()
    {
        // Set distance based on the Node just reached - nextTargetNode
        _controller.distanceToTarget = Grid.instance.GetDistanceBetweenNodes(
            _controller._pathfinder.currentStandingOnNode,
            endNode);

        _pathfinder.path.Remove(nextTargetNode);

        // Did controller move?
        if (endNode != _controller.targetController.GetPrimaryNode())
        {
            FindPathToTarget();
        }

        // Fetch next target node
        if (_pathfinder.path.Count > 0)
        {
            nextTargetNode = _pathfinder.path[0];
        }

        // Based on new nextTargetNode
        IntersectingTarget();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        _targetController = _controller.targetController;

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

        // Didn't find path
        // Do this check last
        else if (_pathfinder.path.Count == 0)
        {
            HandleNoPathToTargetControllerFound();
        }
    }

    protected virtual void IntersectingTarget()
    {
        if (nextTargetNode == null)
        {
            return;
        }

        if (_controller.targetController.IntersectsPoint(nextTargetNode.gridPosPoint))
        {
            ReachTarget();
        }
    }

    void ReachTarget()
    {
        if (_targetController.controllerType == CONTROLLER_TYPE.UNIT)
        {
            ReachedTargetUnit();
        }

        else if (_targetController.controllerType == CONTROLLER_TYPE.BUILDING)
        {
            ReachedTargetBuilding();
        }

        else if (_targetController.controllerType == CONTROLLER_TYPE.STATIC_RESOURCE)
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
            if (_controller._unitStats.isVillager)
            {
                _controller.TransitionToState(_controller.buildState);
            }

            // Special case for Tribe unit reaching Camp building
            else if(_controller._unitStats.isTribe 
                && _targetController == PlayerManager.instance.civilizationCenter)
            {
                _controller.GetComponent<TribeController>().SetupCamp(_targetController.GetComponent<Camp>());
            }

            else
            {
                _controller.TransitionToState(_controller.idleState);
            }
        }

        else
        {
            _controller.TransitionToState(_controller.attackState);
        }
    }

    void ReachedTargetStaticResource()
    {
        if (_controller._unitStats.isVillager)
        {
            _controller.TransitionToState(_controller.gatherState);
        }

        else
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }

    void HandleNoPathToTargetControllerFound()
    {
        // Only seek resources close by if we are close to target resource
        if (_targetController.controllerType == CONTROLLER_TYPE.STATIC_RESOURCE
            && Grid.instance.GetDistanceBetweenNodes(
                _controller._pathfinder.currentStandingOnNode, 
                _targetController.GetPrimaryNode()) <= _controller._unitStats.visionRange * 10)
        {
            if (_controller._unitStats.isVillager)
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