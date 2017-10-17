using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/move to farm")]
public class UnitMoveToFarm : UnitMoveTo
{
    Farm _farm;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _farm = _controller.farm;

        FindPathToTarget();
    }

    protected override void FindPathToTarget()
    {
        endNode = Grid.instance.GetRandomNodeFromController(_farm);
        _pathfinder.FindPath(endNode);
    }

    protected override void ReachedNextTargetNode()
    {
        _pathfinder.path.Remove(nextTargetNode);

        // Fetch next target node
        if (_pathfinder.path.Count > 0)
        {
            nextTargetNode = _pathfinder.path[0];
        }

        // Reached target node
        if (nextTargetNode == endNode)
        {
            _controller.TransitionToState(_controller.farmState);
        }
    }

    public override void CheckTransitions()
    {
        // No path to follow
        if (endNode == null)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}
