﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveTo : UnitState
{
    protected Transform _transform;
    protected Pathfinding _pathfinder;
    protected Vector2 velocity;
    protected Node nextTargetNode;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _controller.isMoving = true;

        // Set up references
        _transform = _controller._transform;
        _pathfinder = _controller._pathfinder;

        velocity = Vector2.zero;
    }

    protected void PlayRunAnimation()
    {

        // Only play if path found
        if (_pathfinder.path.Count > 0
            && !_controller._animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
            _controller._animator.Play("run");
    }

    protected virtual void FindPathToTarget()
    {

    }

    protected void MoveToTarget()
    {
        // Fetch next target node
        if (_pathfinder.path.Count > 0)
            nextTargetNode = _pathfinder.path[0];
        else
            return; // Don't move if path not found

        // Reached next target node
        if (Vector2.Distance(_transform.position, nextTargetNode.worldPosition) < 0.01f)
        {
            _pathfinder.path.Remove(nextTargetNode);

            // Fetch next target node
            if (_pathfinder.path.Count > 0)
            {
                nextTargetNode = _pathfinder.path[0];
            }

            // Another unit is blocking the path
            if (nextTargetNode.unitControllerStandingHere
                && nextTargetNode.unitControllerStandingHere != this)
            {
                UnitStateController unitBlocking = nextTargetNode.unitControllerStandingHere;

                // Wait for blocking unit to find a path around me
                if (!unitBlocking.waitingForNextNodeToGetAvailable && unitBlocking.isMoving)
                {
                    _controller.waitingForNextNodeToGetAvailable = true;
                }

                // Find a path around the blocking unit
                else
                {
                    List<UnitStateController> friendlyStationaryUnits = new List<UnitStateController>(); ;
                    List<UnitStateController> friendlyUnits = PlayerManager.instance.GetAllFriendlyUnits();

                    for (int i = 0; i < friendlyUnits.Count; i++)
                    {
                        if (!friendlyUnits[i].isMoving)
                            friendlyStationaryUnits.Add(friendlyUnits[i]);
                    }

                    FindPathToTargetAvoidingUnits(friendlyStationaryUnits);
                }

                // Wait one tick before continuing
                velocity = Vector2.zero;
                return;
            }

            // Special case for last node
            else if (nextTargetNode.walkable)
            {
                _pathfinder.SetCurrentPathfindingNode(nextTargetNode);
            }
        }

        _controller.waitingForNextNodeToGetAvailable = false;

        float dirx = nextTargetNode.worldPosition.x - _transform.position.x;
        float diry = nextTargetNode.worldPosition.y - _transform.position.y;

        velocity = new Vector2(dirx, diry);
        velocity.Normalize();
    }

    protected void FindPathToTargetAvoidingUnits(List<UnitStateController> unitsToAvoid)
    {
        _pathfinder.unitsToAvoid = unitsToAvoid;

        FindPathToTarget();

        _pathfinder.unitsToAvoid.Clear();
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
