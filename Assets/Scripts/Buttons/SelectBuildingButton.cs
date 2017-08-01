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
        if(WorldManager.Manager.currentUserState == WorldManager.USER_STATE.PLACING_BUILDING)
            WorldManager.Manager.CancelBuildPlacebuild();

        GameObject newBuilding = GameObject.Instantiate(buildingPrefab, WorldManager.Manager.GetMousePosition(), Quaternion.identity) as GameObject;
        WorldManager.Manager.ActivateBuildPlacementState(newBuilding.GetComponent<Building>());
    }
}


