using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class WorldManager : MonoBehaviour {

    // Singleton
    public static WorldManager Manager;

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

    public GameObject clickIndicatorPrefab;

    [HideInInspector]
    public ClickIndicator _clickIndicator;

    [HideInInspector]
    public ObjectSelection _objectSelection;

    [HideInInspector]
    public CursorHoveringUI _cursorHoveringUI;

    [HideInInspector]
    public List<PlayerData> playerData = new List<PlayerData>(4);

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

    Vector2 mousePosition;

    void Awake()
    {
        // Singleton
        Manager = this;

        _clickIndicator = clickIndicatorPrefab.GetComponent<ClickIndicator>();
        _cursorHoveringUI = GetComponent<CursorHoveringUI>();
        _grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        _objectSelection = GetComponent<ObjectSelection>();
        playerData.Add(new PlayerData());
    }

    void Start ()
    {

    }

    public void ActivateBuildPlacementState(Building building)
    {
        currentUserState = USER_STATE.PLACING_BUILDING;
        _clickIndicator.ShowBuildingPlacementindicator(building.GetPosition());
        buildingBeingPlaced = building;
    }

    public void CancelBuildPlacebuild()
    {
        if(buildingBeingPlaced != null)
            buildingBeingPlaced.Destroy();

        currentUserState = USER_STATE.NONE;
        _clickIndicator.ChangeToDefaultCursor();
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
        _clickIndicator.ChangeToDefaultCursor();
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
            _clickIndicator.ShowBuildingPlacementindicator(selectableResource.GetPosition());
        }

        else
        {
            Building hoveringBuilding = _grid.GetBuildingFromWorldPoint(mousePosition);
            if (_objectSelection.GetSelectedGatherers().Count > 0
                && hoveringBuilding != null
                && !hoveringBuilding.constructed
                && !_cursorHoveringUI.IsCursorHoveringUI())
            {
                _clickIndicator.ShowBuildingPlacementindicator(hoveringBuilding.GetPosition());
            }

            else
            {
                _clickIndicator.ChangeToDefaultCursor();
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

        WorldManager.Manager._clickIndicator.ActivateMoveSprite(mousePosition);
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

    public void AddFriendlyUnitReference(UnitStateController unit, int playerID)
    {
        friendlyUnits.Add(unit);
        GetPlayerDataForPlayer(0).population++;
        UpdateHousingText();
    }

    public void RemoveFriendlyUnitReference(UnitStateController unit, int playerID)
    {
        friendlyUnits.Remove(unit);
        GetPlayerDataForPlayer(0).population--;
        UpdateHousingText();
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

    public Grid GetGrid()
    {
        return _grid;
    }

    public PlayerData GetPlayerDataForPlayer(int playerID)
    {
        // Todo: add multiplayer at some stage
        return playerData[0];
    }

    public void UpdateHousingText()
    {
        //UnitUIManager.Manager.housingText.text = new StringBuilder(GetPlayerDataForPlayer(0).population + "/" + GetPlayerDataForPlayer(0).housing).ToString();
    }
}
