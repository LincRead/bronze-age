using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectBuildingButton : UnitUIButton {

    public GameObject buildingPrefab;

    protected override void Start()
    {
        base.Start();
        tooltip = "Build " + title;
    }

    protected override void OnClick()
    {
        base.OnClick();

        // Cancel placement of another building
        PlayerManager.instance.CancelPlaceBuildingState();

        GameObject buildingToPlace = GameObject.Instantiate(buildingPrefab, PlayerManager.mousePosition, Quaternion.identity) as GameObject;
        PlayerManager.instance.SetBuildingPlacementState(buildingToPlace.GetComponent<Building>());
    }
}


