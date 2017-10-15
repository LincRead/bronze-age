using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitMoveToController : UnitMoveTo
{
    protected BaseController _targetController;
    protected Vector2 _targetControllerPosition;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _targetController = _controller.targetController;
        _targetControllerPosition = _targetController.GetPosition();

        if(_targetController.controllerType == CONTROLLER_TYPE.RESOURCE)
        {
            // Remember so we can go back to continue harvesting after derlivering
            // resources to a delivery point
            _controller.lastResourceGatheredPosition = _targetControllerPosition;
        }

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
        if(_targetController == null)
        {
            HandleTargetControllerIsDestroyed();
            return;
        }

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
        if (_targetController.controllerType != CONTROLLER_TYPE.UNIT && !_targetController._basicStats.walkable)
        {
            Grid.instance.SetWalkableValueForTiles(_targetController.GetPosition(), _targetController.size, false);
        }

        // Found path
        if (_pathfinder.path.Count > 0)
        {
            _controller.ignoreControllers.Clear();

            // Ignore unwalkable nodes
            while (_pathfinder.path.Count > 1 && !_pathfinder.path[_pathfinder.path.Count - 1].walkable)
            {
                _pathfinder.path.Remove(_pathfinder.path[_pathfinder.path.Count - 1]);
            }

            // endNode is last node in path
            endNode = _controller._pathfinder.path[_controller._pathfinder.path.Count - 1];

            _controller.targetNode = endNode;

            // Set distance based on first node to end node
            _controller.distanceToTarget = Grid.instance.GetDistanceBetweenNodes(
                _controller._pathfinder.currentStandingOnNode,
                endNode);
        }
    }

    protected override void ReachedNextTargetNode()
    {
        if (_controller.targetController == null)
        {
            HandleTargetControllerIsDestroyed();
            return;
        }

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

        else if (_targetController.controllerType == CONTROLLER_TYPE.RESOURCE)
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
            Building targetBuilding = _targetController.GetComponent<Building>();

            // Not yet constructed
            if (!targetBuilding.constructed)
            {
                if (_controller._unitStats.isVillager)
                {
                    _controller.TransitionToState(_controller.buildState);
                }

                // Special case for Tribe unit reaching Camp building
                else if (_controller._unitStats.isTribe && _targetController == PlayerManager.instance.civilizationCenter)
                {
                    // Need to wait until this value is set to not cause problems in cases where Tribe unit
                    // is placing Tribe Center right next to itself.
                    if(_controller.GetComponent<TribeController>().movingTowarsCamp)
                    {
                        _controller.GetComponent<TribeController>().SetupCamp(_targetController.GetComponent<CivilizationCenter>());
                    }
                }
            }

            else if(_controller._unitStats.isVillager && targetBuilding.title.Equals("Farm"))
            {
                _controller.TransitionToState(_controller.farmState);
            }

            // Drop resources?
            else if(_controller.resoureAmountCarrying > 0 && targetBuilding.resourceDeliveryPoint)
            {
                HandleResourceDrop();
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

    void HandleResourceDrop()
    {
        // Drop resources
        PlayerDataManager.instance.AddResourceForPlayer(
            _controller.resoureAmountCarrying,
            _controller.playerID,
            _controller.resourceTypeCarrying);

        // Go back to farm
        if(_controller.farm != null)
        {
            _controller.MoveTo(_controller.farm);
        }

        // Go back to target resource
        else if (_controller.lastResouceGathered != null)
        {
            _controller.MoveToResource(_controller.lastResouceGathered);
        }

        // Move to the position of the now depleted resource
        else if (_controller.harvestingResource)
        {
            _controller.MoveToResourcePos(_controller.lastResourceGatheredPosition);
        }

        // We clicked manually to deliver resource, so stop at delivery point
        else
        {
            _controller.TransitionToState(_controller.idleState);
        }

        // Reset how much we are carrying
        _controller.resoureAmountCarrying = 0;
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

    void HandleTargetControllerIsDestroyed()
    {
        if (_controller.harvestingResource)
        {
            _controller.MoveToResourcePos(_controller.lastResourceGatheredPosition);
        }

        else
        {
            _controller.MoveTo(_targetControllerPosition);
        }
    }

    protected void HandleNoPathToTargetControllerFound()
    {
        if (_targetController == null)
        {
            HandleTargetControllerIsDestroyed();
        }

        // Only seek resources close by if we are close to target resource
        else if (_targetController.controllerType == CONTROLLER_TYPE.RESOURCE
            && Grid.instance.GetDistanceBetweenNodes(
                _controller._pathfinder.currentStandingOnNode, 
                _targetController.GetPrimaryNode()) <= _controller._unitStats.visionRange * 10)
        {
            if (_controller._unitStats.isVillager)
            {
                _controller.SeekClosestResource(_controller.resourceTitleCarrying);
            }
        }

        else if(timeSinceRouteBlocked >= timeBeforeFindingNewRoute)
        {
            FindPathToTarget();
            timeSinceRouteBlocked = 0.0f;
        }
    }
}