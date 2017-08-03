using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum BUILDINGS
{
    VILLAGE_CENTER
}

public class SelectBuildingButton : UnitUIButton {

    public GameObject buildingPrefab;
    //Building buildingScript;

    protected override void Start()
    {
        base.Start();
        tooltip = "Build " + title;

        //buildingScript = buildingPrefab.GetComponent<Building>();
    }

    protected override void OnClick()
    {
        if(WorldManager.instance.currentUserState == WorldManager.USER_STATE.PLACING_BUILDING)
            WorldManager.instance.CancelBuildPlacebuild();

        GameObject newBuilding = GameObject.Instantiate(buildingPrefab, WorldManager.instance.GetMousePosition(), Quaternion.identity) as GameObject;
        WorldManager.instance.ActivateBuildPlacementState(newBuilding.GetComponent<Building>());
    }
}


