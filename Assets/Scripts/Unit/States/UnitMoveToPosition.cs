using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/move to position")]
public class UnitMoveToPosition : UnitState
{
    Transform _transform;
    Pathfinding _pathfinder;
    Vector2 velocity;
    Node targetNode;
    Node nextTargetNode;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        // Set up references
        _transform = _controller._transform;
        _pathfinder = _controller._pathfinder;

        velocity = Vector2.zero;

        FindPathToPosition();
        
        // Only play if path found
        if (_pathfinder.path.Count > 0
            && !controller._animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
            _controller._animator.Play("run");
    }

    void FindPathToPosition()
    {
        targetNode = _pathfinder.GetNodeFromPoint(_controller.targetPosition);

        if (targetNode == null)
            return;

        // TODO: find closest available node as target instead

        // TODO: clicking on a tile unit is standing on at the same time as harvesting, 
        // unit needs to go to middle of tile ..... what did I mean here!???

        if (!targetNode.walkable)
            return;

        _pathfinder.FindPath(targetNode);
    }

    public override void DoActions()
    {
        _pathfinder.DetectCurrentPathfindingNode(_transform.position);

        MoveToTarget();

        _controller.ExecuteMovement(velocity);
        _controller.FaceMoveDirection(velocity);
    }

    void MoveToTarget()
    {
        // Fetch next target node
        if (_pathfinder.path.Count > 0)
            nextTargetNode = _pathfinder.path[0];
        else
            return; // Don't move if path not found

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
                FindPathToPosition();
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
        // No path to follow
        if (_pathfinder.path == null || _pathfinder.path.Count == 0)
            _controller.TransitionToState(_controller.idleState);

        // Reached target node
        else if (_pathfinder.currentStandingOnNode == targetNode)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }

    public override void OnExit()
    {
        _pathfinder.path.Clear();
    }
}