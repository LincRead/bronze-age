﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class WorldManager : MonoBehaviour {

    [HideInInspector]
    public static int myPlayerID = 0;

    // 1 for singleplayer, more for multiplayer
    public int numPlayers = 1;

    [HideInInspector]
    public Grid _grid;

    public enum USER_STATE
    {
        NONE,
        PLACING_BUILDING,
        MENU
    }

    [HideInInspector]
    public USER_STATE currentUserState = USER_STATE.NONE;

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

    private static WorldManager worldManager;

    public static WorldManager instance
    {
        get
        {
            if (!worldManager)
            {
                worldManager = FindObjectOfType(typeof(WorldManager)) as WorldManager;

                if (!worldManager)
                {
                    Debug.LogError("There needs to be one active WorldManager script on a GameObject in your scene.");
                }
                else
                {
                    worldManager.Init();
                }
            }

            return worldManager;
        }
    }

    void Init()
    {
        _cursorHoveringUI = GetComponent<CursorHoveringUI>();
        _grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        _objectSelection = GetComponent<ObjectSelection>();
    }

    public void ActivateBuildPlacementState(Building building)
    {
        currentUserState = USER_STATE.PLACING_BUILDING;
        buildingBeingPlaced = building;
        EventManager.TriggerEvent("SetBuildCursor");
    }

    public void CancelBuildPlacebuild()
    {
        if(buildingBeingPlaced != null)
            buildingBeingPlaced.Destroy();

        currentUserState = USER_STATE.NONE;
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

        currentUserState = USER_STATE.NONE;
        EventManager.TriggerEvent("SetDefaultCursor");
    }

    // Update is called once per frame
    void Update () {

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch (currentUserState)
        {
            case USER_STATE.NONE:
                UpdateMouseCursor();
                if (Input.GetMouseButtonUp(1))
                    MoveToTarget();
                break;

            case USER_STATE.PLACING_BUILDING:
                HandlePlaceBuilding();
                break;

            default:
                break;
        }
    }

    void HandlePlaceBuilding() { }

    void UpdateMouseCursor()
    {
        Resource mouseHoveringNodeWithResource = _grid.GetResourceFromWorldPoint(mousePosition);

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
            Building hoveringBuilding = _grid.GetBuildingFromWorldPoint(mousePosition);
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

    public void MoveToTarget()
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

        goToBuldingForTask = GetBuildingAtWorldPosition(mousePosition);

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

    public Building GetBuildingAtWorldPosition(Vector2 pos)
    {
        Tile mouseOverNode = _grid.GetTileFromWorldPoint(mousePosition);
        if (mouseOverNode == null)
            return null;

        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].IntersectsPoint(mouseOverNode.gridPosPoint))
            {
                return buildings[i];
            }
        }

        return null;
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

    public void AddBuildingReference(Building building)
    {
        buildings.Add(building);
    }

    public void RemoveBuildingReference(Building building)
    {
        buildings.Remove(building);
    }

    public void AddResourceReference(Resource resource)
    {
        resources.Add(resource);
    }

    public void RemoveResourceReference(Resource resource)
    {
        _grid.RemoveTilesOccupiedByResource(resource);
        resources.Remove(resource);
    }

    public List<UnitStateController> GetFriendlyUnits()
    {
        return friendlyUnits;
    }

    public List<Building> GetAllBuildings()
    {
        return buildings;
    }

    public Vector2 GetMousePosition()
    {
        return mousePosition;
    }

    public void UpdateHousingText()
    {
        //UnitUIManager.Manager.housingText.text = new StringBuilder(GetPlayerDataForPlayer(0).population + "/" + GetPlayerDataForPlayer(0).housing).ToString();
    }
}
