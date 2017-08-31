using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class VillagerProductionButton : ProductionButton
{
    protected override void OnClick()
    {
        base.OnClick();

        // Cancel placement of another building
        PlayerManager.instance.CancelPlaceBuildingState();

        GameObject buildingToPlace = GameObject.Instantiate(prefab, PlayerManager.mousePosition, Quaternion.identity) as GameObject;
        PlayerManager.instance.SetBuildingPlacementState(buildingToPlace.GetComponent<Building>());
    }
}
