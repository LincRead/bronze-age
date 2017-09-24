using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/gather")]
public class UnitGather : UnitState {

    Resource _resource;

    float harvestProgress = 0.0f;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        harvestProgress = 0.0f;

        _resource = _controller.targetController.GetComponent<Resource>();

        PlayGatherAnimation();
    }

    void PlayGatherAnimation()
    {
        // Based on type of resource
        switch (_resource.harvestType)
        {
            case HARVEST_TYPE.GATHER:
                _controller._animator.Play("gather");
                break;
            case HARVEST_TYPE.CHOP:
                _controller._animator.Play("chop");
                break;
            case HARVEST_TYPE.FARM:
                _controller._animator.Play("farm");
                break;
            case HARVEST_TYPE.MINE:
                _controller._animator.Play("mine");
                break;
        }
    }

    public override void DoActions()
    {
        // Todo add harvest rate
        harvestProgress += 1.0f * Time.deltaTime;

        // Make sure it hasn't been depleted when we add a resouce to harvester
        if (harvestProgress >= _resource.harvestDifficulty 
            && !_resource.depleted)
        {
            _resource.Harvest();

            harvestProgress = 0.0f;

            // Remember so we can go back to continue harvesting after derlivering
            // resources to a delivery point
            _controller.lastResouceGathered = _resource;
            _controller.lastResourceGatheredPosition = _resource.GetPosition();

            if(_controller.resourceTypeCarrying != _resource.resourceType)
            {
                // Reset
                _controller.resoureAmountCarrying = 0;
                _controller.resourceTypeCarrying = _resource.resourceType;
                _controller.resourceTitleCarrying = _resource.title;
            }

            _controller.resoureAmountCarrying++;
        }
    }

    public override void CheckTransitions()
    {
        // Todo define value for max carry amount
        if(_controller.resoureAmountCarrying >= 2)
        {
            _controller.seekClosestResourceDeliveryPoint();
        }

        else if(_resource.depleted)
        {
            _controller.SeekClosestResource(_resource.title);
        }
    }
}
