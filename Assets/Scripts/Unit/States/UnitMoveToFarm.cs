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

    public override void CheckTransitions()
    {
        // Reached target node
        if (nextTargetNode == endNode
            && Vector2.Distance(_transform.position, endNode.worldPosition) < 0.01f)
        {
            _controller.TransitionToState(_controller.farmState);
        }

        // No path to follow
        if (endNode == null)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}
