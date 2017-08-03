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
        if(PlayerManager.instance.currentUserState == PlayerManager.PLAYER_ACTION_STATE.PLACING_BUILDING)
            PlayerManager.instance.CancelBuildPlacebuild();

        GameObject newBuilding = GameObject.Instantiate(buildingPrefab, PlayerManager.mousePosition, Quaternion.identity) as GameObject;
        PlayerManager.instance.ActivateBuildPlacementState(newBuilding.GetComponent<Building>());
    }
}


