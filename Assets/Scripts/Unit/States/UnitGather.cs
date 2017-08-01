using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/gather")]
public class UnitGather : UnitState {

    Resource _resource;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _resource = _controller.targetController.GetComponent<Resource>();

        PlayGatherAnimation();
    }

    void PlayGatherAnimation()
    {
        // Based on type of resource
        switch (_resource.resourceType)
        {
            case Resource.HARVEST_TYPE.GATHER:
                _controller._animator.Play("harvest");
                break;
            case Resource.HARVEST_TYPE.CHOP:
                _controller._animator.Play("chop");
                break;
            case Resource.HARVEST_TYPE.FARM:
                _controller._animator.Play("farm");
                break;
            case Resource.HARVEST_TYPE.MINE:
                _controller._animator.Play("mine");
                break;
        }
    }

    public override void DoActions()
    {
        // Todo different harvest rates
        _resource.Harvest(1, _controller.playerID);
    }

    public override void CheckTransitions()
    {
        if(_resource.depleted)
        {
            _controller.TransitionToState(_controller.idleState);
        }
    }
}
