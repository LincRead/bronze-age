using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PlayerManager : MonoBehaviour {

    [HideInInspector]
    public static int myPlayerID = 0;

    public enum PLAYER_ACTION_STATE
    {
        NONE,
        PLACING_BUILDING,
        MENU
    }

    [HideInInspector]
    public PLAYER_ACTION_STATE currentUserState = PLAYER_ACTION_STATE.NONE;

    [HideInInspector]
    public ObjectSelection _objectSelection;

    [HideInInspector]
    public CursorHoveringUI _cursorHoveringUI;

    [HideInInspector]
    public List<UnitStateController> friendlyUnits = new List<UnitStateController>();

    [HideInInspector]
    public List<Building> buildings = new List<Building>();

    [HideInInspector]
    public List<Resource> resources = new List<Resource>();

    [HideInInspector]
    public Building buildingBeingPlaced = null;

    [HideInInspector]
    public Resource selectableResource = null;

    [HideInInspector]
    public Resource mouseHoveringResource = null;

    [HideInInspector]
    public static Vector2 mousePosition;

    private static PlayerManager playerManager;

    public static PlayerManager instance
    {
        get
        {
            if (!playerManager)
            {
                playerManager = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;

                if (!playerManager)
                {
                    Debug.LogError("There needs to be one active PlayerManager script on a GameObject in your scene.");
                }
                else
                {
                    playerManager.Init();
                }
            }

            return playerManager;
        }
    }

    void Init()
    {
        _cursorHoveringUI = GetComponent<CursorHoveringUI>();
        _objectSelection = GetComponent<ObjectSelection>();
    }

    public void ActivateBuildPlacementState(Building building)
    {
        currentUserState = PLAYER_ACTION_STATE.PLACING_BUILDING;
        buildingBeingPlaced = building;
        EventManager.TriggerEvent("SetBuildCursor");
    }

    public void CancelBuildPlacebuild()
    {
        if(buildingBeingPlaced != null)
            buildingBeingPlaced.CancelPlacing();

        currentUserState = PLAYER_ACTION_STATE.NONE;
        EventManager.TriggerEvent("SetDefaultCursor");
    }

    public void StopBuildPlacementState(Building building)
    {
        if(building != null)
        {
            List<UnitStateController> selectedBuilders = _objectSelection.GetSelectedGatherers();
            for (int i = 0; i < selectedBuilders.Count; i++)
            {
                selectedBuilders[i].MoveTo(building);
            }
        }

        currentUserState = PLAYER_ACTION_STATE.NONE;
        EventManager.TriggerEvent("SetDefaultCursor");
    }

    void Update ()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch (currentUserState)
        {
            case PLAYER_ACTION_STATE.NONE:
                UpdateMouseCursor();
                if (Input.GetMouseButtonUp(1))
                    MoveSelectedUnitsToTarget();
                break;

            case PLAYER_ACTION_STATE.PLACING_BUILDING:
                HandlePlaceBuilding();
                break;

            default:
                break;
        }
    }

    void HandlePlaceBuilding()
    {
        // ...
    }

    void UpdateMouseCursor()
    {
        Resource mouseHoveringNodeWithResource = Grid.instance.GetResourceFromWorldPoint(mousePosition);

        if (mouseHoveringNodeWithResource == null)
            selectableResource = mouseHoveringResource;
        else
            selectableResource = mouseHoveringNodeWithResource;

        if (selectableResource != null
            && _objectSelection.GetSelectedGatherers().Count > 0
            && !_cursorHoveringUI.IsCursorHoveringUI())
        {
            // Todo change based on type of resource
            EventManager.TriggerEvent("SetBuildCursor");
        }

        else
        {
            Building hoveringBuilding = Grid.instance.GetBuildingFromWorldPoint(mousePosition);
            if (_objectSelection.GetSelectedGatherers().Count > 0
                && hoveringBuilding != null
                && !hoveringBuilding.constructed
                && !_cursorHoveringUI.IsCursorHoveringUI())
            {
                EventManager.TriggerEvent("SetBuildCursor");
            }

            else
            {
                EventManager.TriggerEvent("SetDefaultCursor");
            }
        }
    }

    public void MoveSelectedUnitsToTarget()
    {
        float averageX = 0;
        float averageY = 0;
        int numUnits = 0;
        Building goToBuldingForTask = null;

        for (int i = 0; i < friendlyUnits.Count; i++)
        {
            if (friendlyUnits[i].selected)
            {
                averageX += friendlyUnits[i]._transform.position.x;
                averageY += friendlyUnits[i]._transform.position.y;
                numUnits++;
            }
        }

        if (numUnits == 0)
            return;

        averageX /= numUnits;
        averageY /= numUnits;

        goToBuldingForTask = FindBuildingAtWorldPosition();

        // Todo refactor dont check null for every unit
        for (int i = 0; i < friendlyUnits.Count; i++)
        {
            if(friendlyUnits[i].selected) 
            {
                if(goToBuldingForTask != null)
                {
                    friendlyUnits[i].MoveTo(goToBuldingForTask);
                }

                else if(selectableResource != null)
                {
                    friendlyUnits[i].MoveTo(selectableResource);
                }

                else
                {
                    Vector2 offset = new Vector2(averageX - friendlyUnits[i]._transform.position.x, averageY - friendlyUnits[i]._transform.position.y);
                    friendlyUnits[i].MoveTo(mousePosition - offset);
                }
            }
        }

        EventManager.TriggerEvent("ActivateMoveUnitsIndicator");
    }

    public Building FindBuildingAtWorldPosition()
    {
        Tile mouseOverNode = Grid.instance.GetTileFromWorldPoint(mousePosition);
        if (mouseOverNode == null)
            return null;

        return mouseOverNode.buildingOccupying;
    }

    public void AddFriendlyUnitReference(UnitStateController unit, int player)
    {
        friendlyUnits.Add(unit);
        PlayerDataManager.instance.AddPopulationForPlayer(1, player);
    }

    public void RemoveFriendlyUnitReference(UnitStateController unit, int player)
    {
        friendlyUnits.Remove(unit);
        PlayerDataManager.instance.AddPopulationForPlayer(-1, player);
    }

    public List<UnitStateController> GetFriendlyUnits()
    {
        return friendlyUnits;
    }
}
