using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/gather")]
public class UnitGather : UnitState {

    Resource _resource;
    PlayerData data;

    float harvestProgress = 0.0f;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        data = PlayerDataManager.instance.GetPlayerData(_controller.playerID);

        harvestProgress = 0.0f;

        _resource = _controller.targetController.GetComponent<Resource>();

        // Remember so we can go back to continue harvesting after derlivering
        // resources to a delivery point
        _controller.lastResouceGathered = _resource;

        if (_controller.resoureAmountCarrying < data.villagerCarryLimit)
        {
            PlayGatherAnimation();
        }
    }

    void PlayGatherAnimation()
    {
        // Based on type of resource
        switch (_resource.harvestType)
        {
            case HARVEST_TYPE.GATHER_BERRIES:
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
        // Based on type of resource
        switch (_resource.harvestType)
        {
            case HARVEST_TYPE.GATHER_BERRIES:
                harvestProgress += PlayerDataManager.instance.GetPlayerData(_controller.playerID).gatherBerriesSpeed * Time.deltaTime;
                break;
            case HARVEST_TYPE.GATHER_MEAT:
                harvestProgress += PlayerDataManager.instance.GetPlayerData(_controller.playerID).gatherMeatSpeed * Time.deltaTime;
                break;
            case HARVEST_TYPE.FISHING:
                harvestProgress += PlayerDataManager.instance.GetPlayerData(_controller.playerID).fishingSpeed * Time.deltaTime;
                break;
            case HARVEST_TYPE.CHOP:
                harvestProgress += PlayerDataManager.instance.GetPlayerData(_controller.playerID).woodCuttingSpeed * Time.deltaTime;
                break;
            case HARVEST_TYPE.FARM:
                harvestProgress += PlayerDataManager.instance.GetPlayerData(_controller.playerID).farmingSpeed * Time.deltaTime;
                break;
            case HARVEST_TYPE.MINE:
                harvestProgress += PlayerDataManager.instance.GetPlayerData(_controller.playerID).miningSpeed * Time.deltaTime;
                break;
        }

        // Make sure it hasn't been depleted when we add a resouce to harvester
        if (harvestProgress >= _resource.harvestDifficulty && !_resource.depleted)
        {
            _resource.Harvest();

            harvestProgress = 0.0f;

            if (_controller.resourceTypeCarrying != _resource.resourceType)
            {
                CarryNewResource();
            }

            _controller.resoureAmountCarrying++;
        }
    }

    void CarryNewResource()
    {
        _controller.resoureAmountCarrying = 0;
        _controller.resourceTypeCarrying = _resource.resourceType;
    }

    public override void CheckTransitions()
    {
        // Halved carry limit for metal
        if(_resource.resourceType == RESOURCE_TYPE.METAL)
        {
            if (_controller.resoureAmountCarrying >= (data.villagerCarryLimit / 2))
            {
                _controller.seekClosestResourceDeliveryPoint();
                return;
            }
        }

        // Go back with harvested resources
        else if (_controller.resoureAmountCarrying >= data.villagerCarryLimit)
        {
            _controller.seekClosestResourceDeliveryPoint();
            return;
        }

        if(_resource.depleted)
        {
            // Don't reliver if not carrying any resources
            if (_controller.resoureAmountCarrying == 0)
            {
                _controller.SeekClosestResource(_controller.resourceTitleCarrying);
            }   

            // Deliver back whatever we were able to gather by the time resource depleted
            else
            {
                _controller.seekClosestResourceDeliveryPoint();
            }
        }
    }
}
