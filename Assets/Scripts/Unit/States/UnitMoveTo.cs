using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveTo : UnitState
{
    protected Transform _transform;
    protected Pathfinding _pathfinder;
    protected Vector2 velocity;
    protected Node nextTargetNode;
    protected Node endNode;

    protected float timeSinceRouteBlocked = 0.0f;
    protected float timeBeforeGivingUpRoute = 0.5f;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _controller.isMoving = true;

        // Set up references
        _transform = _controller._transform;
        _pathfinder = _controller._pathfinder;

        velocity = Vector2.zero;
    }

    protected override void PlayAnimation()
    {
        // Only play if path found
        if (_pathfinder.path.Count > 0)
        {
            // Carrying resource


            // Not carrying resources
            if (!_controller._animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
            {
                _controller._animator.Play("run", -1, 0.0f);
            }
        }
    }

    protected virtual void FindPathToTarget()
    {

    }

    protected void MoveToTarget()
    {
        // Fetch next target node
        if (_pathfinder.path.Count > 0)
        {
            nextTargetNode = _pathfinder.path[0];
        }

        else
        {
            velocity = Vector2.zero;
            return;
        }

        // Reached next target node
        if (Vector2.Distance(_transform.position, nextTargetNode.worldPosition) < 0.01f)
        {
            ReachedNextTargetNode();
            _controller.UpdateVisibility();
        }

        // Another unit is blocking the path
        else if (nextTargetNode.unitControllerStandingHere && nextTargetNode.unitControllerStandingHere != _controller)
        {
            UnitStateController unitBlocking = nextTargetNode.unitControllerStandingHere;

            // Find a path around the unit moving if it is blocking us
            if (unitBlocking.isMoving)
            {
                FindPathToTargetAvoidingUnit(unitBlocking);
            }

            // Find path around all stationary units
            else
            {
                FindPathToTarget();
            }

            WaitToMove();
            return;
        }

        else if (nextTargetNode.walkable)
        {
            _pathfinder.SetCurrentPathfindingNode(nextTargetNode);
        }

        timeSinceRouteBlocked = 0.0f;

        float dirx = nextTargetNode.worldPosition.x - _transform.position.x;
        float diry = nextTargetNode.worldPosition.y - _transform.position.y;

        velocity = new Vector2(dirx, diry);
        velocity.Normalize();
    }

    void WaitToMove()
    {
        timeSinceRouteBlocked += Time.deltaTime;
        velocity = Vector2.zero;
    }

    protected virtual void ReachedNextTargetNode()
    {
        _pathfinder.path.Remove(nextTargetNode);

        // Fetch next target node
        if (_pathfinder.path.Count > 0)
        {
            nextTargetNode = _pathfinder.path[0];
        }
    }

    protected void FindPathToTargetAvoidingUnit(UnitStateController unitToAvoid)
    {
        _pathfinder.unitToAvoid = unitToAvoid;

        FindPathToTarget();

        _pathfinder.unitToAvoid = null;
    }

    public override void DoActions()
    {
        MoveToTarget();

        _controller.ExecuteMovement(velocity);
        _controller.FaceMoveDirection(velocity);
    }

    public override void OnExit()
    {
        _pathfinder.path.Clear();
        _controller.isMoving = false;
    }
}
