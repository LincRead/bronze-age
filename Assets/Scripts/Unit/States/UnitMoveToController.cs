using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/move to controller")]
public class UnitMoveToController : UnitState
{
    Transform _transform;
    Pathfinding _pathfinder;
    BaseController _targetObject;

    Vector2 velocity;
    Vector3 lastPosition;

    Node nextTargetNode;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        // Set up references
        _transform = _controller._transform;
        _pathfinder = _controller._pathfinder;
        _targetObject = _controller.targetController;

        velocity = Vector2.zero;

        FindPathToTarget();

        if (!controller._animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
            _controller._animator.Play("run");
    }

    void FindPathToTarget()
    {
       _pathfinder.DetectCurrentPathfindingNode(_transform.position);

        // Make sure unit can use pathfinding to get to object
        WorldManager.Manager.GetGrid().SetWalkableValueForTiles(_targetObject, true);

        // Feedback to player about action
        WorldManager.Manager._clickIndicator.ActivateBounceEffect();

        // Find path
        Node node = _pathfinder.GetNodeFromPoint(_targetObject.GetPrimaryNode().worldPosition);

        if (node != null)
            _pathfinder.FindPath(node);

        // Reset for pathfinding
        WorldManager.Manager.GetGrid().SetWalkableValueForTiles(_targetObject.GetPosition(), _targetObject.size, false);
    }

    public override void DoActions()
    {
        _pathfinder.DetectCurrentPathfindingNode(_transform.position);

        if (_pathfinder.currentStandingOnNode.walkable)
            lastPosition = _transform.position;

        MoveToTarget();

        _controller.ExecuteMovement(velocity);
        _controller.FaceMoveDirection(velocity);
    }

    void MoveToTarget()
    {
        // Fetch next target node
        if (_pathfinder.path.Count > 0)
            nextTargetNode = _pathfinder.path[0];

        // Reached next target node
        if (_pathfinder.currentStandingOnNode == nextTargetNode)
        {
            _pathfinder.path.Remove(nextTargetNode);
            
            // Fetch next target node
            if (_pathfinder.path.Count > 0)
            {
                nextTargetNode = _pathfinder.path[0];
            }
        }

        // Another unit is blocking the path
        if (nextTargetNode.unitControllerStandingHere && nextTargetNode.unitControllerStandingHere != this)
        {
            // Wait for blocking unit to find a path around me
            if (!nextTargetNode.unitControllerStandingHere.waitingForNextNodeToGetAvailable)
            {
                _controller.waitingForNextNodeToGetAvailable = true;
            }

            // Find a path around the blocking unit
            else
            {
                FindPathToTarget();
            }

            // Wait one tick before continuing
            velocity = Vector2.zero;
            return;
        }

        _controller.waitingForNextNodeToGetAvailable = false;

        float dirx = nextTargetNode.worldPosition.x - _transform.position.x;
        float diry = nextTargetNode.worldPosition.y - _transform.position.y;

        velocity = new Vector2(dirx, diry);
        velocity.Normalize();
    }

    public override void CheckTransitions()
    {
        if (_pathfinder.path == null)
            _controller.TransitionToState(_controller.idleState);

        if (_controller.targetController.IntersectsPoint(_pathfinder.currentStandingOnNode.gridPosPoint))
        {
            // Don't wanna stay on an unwalkable node, so move back again
            if (!_pathfinder.currentStandingOnNode.walkable)
                _transform.position = lastPosition;

            // TODO resources and building
            if (_controller._unitStats.builder && _targetObject.controllerType == BaseController.CONTROLLER_TYPE.BUILDING)
            {
                _controller.TransitionToState(_controller.buildState);
            }

            else if (_controller._unitStats.gatherer && _targetObject.controllerType == BaseController.CONTROLLER_TYPE.STATIC_RESOURCE)
            {
                _controller.TransitionToState(_controller.gatherState);
            }

            else
            {
                _controller.TransitionToState(_controller.idleState);
            }
        }
    }

    public override void OnExit()
    {
        _pathfinder.path.Clear();
    }
}