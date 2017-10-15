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

        // If we reach main civilization building, make sure don't do any construction work
        // Repair work is allowed
        if(_building._buildingStats.isCivilizationCenter && !_building.constructed)
        {
            _controller.TransitionToState(_controller.idleState);
            return;
        }

        if(!_building.constructed || _building.hitpointsLeft < _building.maxHitPoints)
        {
            _controller._animator.Play("build");
        }
    }

    public override void DoActions()
    {
        if (_building != null)
        {
            _building.Build(1);
        }
    }

    public override void CheckTransitions()
    {
        // Todo: add being destroyed value
        if(_building == null)
        {
            _controller.TransitionToState(_controller.idleState);
        }

        if (_building.constructed && _building.hitpointsLeft == _building.maxHitPoints)
        {
            if(_controller._unitStats.isVillager 
                && _building.title.Equals("Farm") 
                && !_building.GetComponent<Farm>().hasFarmer)
            {
                // Set as farmer
                Farm farm = _building.GetComponent<Farm>();
                _controller.farm = farm;
                farm.hasFarmer = true;

                _controller.TransitionToState(_controller.farmState);
            }

            else
            {
                _controller.TransitionToState(_controller.idleState);
            }

        }
    }

    public override void OnExit()
    {

    }
}