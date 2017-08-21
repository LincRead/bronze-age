using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/build")]
public class UnitBuild : UnitState
{
    Building _building;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _building = _controller.targetController.GetComponent<Building>();

        if(!_building.constructed || _building.hitpointsLeft < _building.maxHitPoints)
            _controller._animator.Play("build");
    }

    public override void DoActions()
    {
        _building.Build(1); // todo: * efficiency
    }

    public override void CheckTransitions()
    {
        if (_building.constructed && _building.hitpointsLeft == _building.maxHitPoints)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }

    public override void OnExit()
    {

    }
}