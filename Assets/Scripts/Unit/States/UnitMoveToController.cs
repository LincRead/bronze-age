using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "States/Unit states/move to controller")]
public class UnitMoveToController : UnitMoveTo
{
    protected BaseController _targetObject;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _targetObject = _controller.targetController;

        FindPathToTarget();
        PlayRunAnimation();
    }

    protected override void FindPathToTarget()
    {
       _pathfinder.DetectCurrentPathfindingNode(_transform.position);

        // Make sure unit can use pathfinding to get to object
        Grid.instance.SetWalkableValueForTiles(_targetObject, true);

        // Find path
        Node node = _pathfinder.GetNodeFromPoint(_targetObject.GetPrimaryNode().worldPosition);

        if (node != null)
            _pathfinder.FindPath(node);

        // Reset for pathfinding
        Grid.instance.SetWalkableValueForTiles(_targetObject.GetPosition(), _targetObject.size, false);
    }

    public override void CheckTransitions()
    {
        if (_pathfinder.path == null || _pathfinder.path.Count == 0)
        {
            _controller.TransitionToState(_controller.idleState);
        }

        if (_controller.targetController.IntersectsPoint(nextTargetNode.gridPosPoint))
        { 
            if (_controller._unitStats.builder && _targetObject.controllerType == BaseController.CONTROLLER_TYPE.BUILDING)
            {
                _controller.TransitionToState(_controller.buildState);
            }

            else if (_controller._unitStats.gatherer && _targetObject.controllerType == BaseController.CONTROLLER_TYPE.STATIC_RESOURCE)
            {
                _controller.TransitionToState(_controller.gatherState);
            }

            // Todo attack mode

            else
            {
                _controller.TransitionToState(_controller.idleState);
            }
        }
    }
}