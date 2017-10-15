using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/farm")]
public class UnitFarm : UnitState
{
    Farm _farm;
    PlayerData data;

    float harvestProgress = 0.0f;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        data = PlayerDataManager.instance.GetPlayerData(_controller.playerID);

        harvestProgress = 0.0f;

        // Remember so we can go back to continue harvesting after delivering
        // resources to a delivery point
         _farm = _controller.farm;

        if (_controller.resoureAmountCarrying < data.villagerCarryLimit)
        {
            PlayGatherAnimation();
        }
    }

    protected void PlayGatherAnimation()
    {
        _controller._animator.Play("gather");
    }

    public override void DoActions()
    {
        harvestProgress += PlayerDataManager.instance.GetPlayerData(_controller.playerID).farmingSpeed * Time.deltaTime;

        // Make sure it hasn't been depleted when we add a resouce to harvester
        if (harvestProgress >= _farm.harvestDifficulty)
        {
            harvestProgress = 0.0f;

            if (_controller.resourceTypeCarrying != RESOURCE_TYPE.CROPS)
            {
                    _controller.resoureAmountCarrying = 0;
                    _controller.resourceTypeCarrying = RESOURCE_TYPE.CROPS;
            }

            _controller.resoureAmountCarrying++;
        }
    }

    public override void CheckTransitions()
    {
        if (_controller.resoureAmountCarrying >= data.villagerCarryLimit)
        {
            // TODO seek closest GRAIN OR TOWN CENTER
            _controller.seekClosestResourceDeliveryPoint();
            return;
        }
    }
}
