using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PlayerManager : MonoBehaviour {

    [HideInInspector]
    public static int myPlayerID = 0;

    // 0 - Tribe
    // 1 - Settlement
    // 2 - City-State
    public int currentAge = 0;

    public enum PLAYER_ACTION_STATE
    {
        DEFAULT,
        ATTACK_MOVE,
        PLACING_BUILDING,
        SETTING_RALLY_POINT,
        MENU
    }

    struct MouseHoveringController
    {
        public int priorityValue;
        public BaseController controller;
    }

    [HideInInspector]
    MouseHoveringController mouseHoveringController;

    [HideInInspector]
    public PLAYER_ACTION_STATE currentUserState = PLAYER_ACTION_STATE.DEFAULT;

    [HideInInspector]
    public ControllerSelecting _controllerSelecting;

    [HideInInspector]
    public List<UnitStateController> friendlyUnits = new List<UnitStateController>();

    [HideInInspector]
    public Building buildingBeingPlaced = null;

    [HideInInspector]
    public BaseController selectableController = null;

    [HideInInspector]
    public CivilizationCenter civilizationCenter = null;

    [HideInInspector]
    public Tile selectedTile = null;

    [HideInInspector]
    public static Vector2 mousePosition;

    // Show rally point for buildings with rally point
    public GameObject rallyPointSprite;

    private static PlayerManager playerManager;

    // Food intake ui update
    float timeSinceLastFoodUIUpdate = 0.0f;
    float timeBetweenFoodUIUpdates = 1.0f;
    float timePerFoodIntakePoint = 30f;

    public static PlayerManager instance
    {
        get
        {
            if (!playerManager)
            {
                playerManager = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;

                if (!playerManager)
                {
                    Debug.LogError("There needs to be one active PlayerManager script on a GameObject in the scene.");
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
        _controllerSelecting = GetComponent<ControllerSelecting>();
        mouseHoveringController = new MouseHoveringController();
        rallyPointSprite.SetActive(false);
    }

    public void SetBuildingPlacementState(Building building)
    {
        buildingBeingPlaced = building;
        buildingBeingPlaced.GetComponent<Building>().playerID = PlayerManager.myPlayerID;
        currentUserState = PLAYER_ACTION_STATE.PLACING_BUILDING;
        EventManager.TriggerEvent("SetBuildCursor");
    }

    public void CancelPlaceBuildingState()
    {
        if(buildingBeingPlaced != null)
        {
            buildingBeingPlaced.CancelPlacing();
        }

        buildingBeingPlaced = null;

        currentUserState = PLAYER_ACTION_STATE.DEFAULT;
        EventManager.TriggerEvent("SetDefaultCursor");
    }

    public void SetRallyPointState()
    {
        currentUserState = PLAYER_ACTION_STATE.SETTING_RALLY_POINT;
        EventManager.TriggerEvent("SetRallyPointCursor");
    }

    public void CancelRallyPointState()
    {
        currentUserState = PLAYER_ACTION_STATE.DEFAULT;
        EventManager.TriggerEvent("SetDefaultCursor");
    }

    public void PlacedBuilding(BaseController building)
    {
        List<UnitStateController> selectedBuilders = _controllerSelecting.GetSelectedGatherers();

        for (int i = 0; i < selectedBuilders.Count; i++)
        {
            selectedBuilders[i].MoveTo(building);
        }

        buildingBeingPlaced = null;

        currentUserState = PLAYER_ACTION_STATE.DEFAULT;
    }

    public void SetAttackMoveState()
    {
        currentUserState = PLAYER_ACTION_STATE.ATTACK_MOVE;
        EventManager.TriggerEvent("SetAttackCursor");
    }

    public void SetDefaultState()
    {
        EventManager.TriggerEvent("SetDefaultCursor");
        currentUserState = PLAYER_ACTION_STATE.DEFAULT;
    }

    void Update ()
    {
        UpdateFoodIntake();
        UpdateNewCitizens();

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch (currentUserState)
        {
            case PLAYER_ACTION_STATE.DEFAULT:

                UpdateSelectableController();
                UpdateMouseCursor();

                if (Input.GetMouseButtonUp(0))
                {
                    UpdateMouseCursor();
                }

                if (Input.GetMouseButtonUp(1))
                {
                    MoveSelectedUnitsToNewTarget(false);
                }

                break;

            case PLAYER_ACTION_STATE.ATTACK_MOVE:

                if (!CursorHoveringUI.value)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        SetDefaultState();
                        return;
                    }

                    else if(Input.GetMouseButtonUp(1))
                    {
                        MoveSelectedUnitsToNewTarget(true);
                        SetDefaultState();
                    }
                }

                break;

            case PLAYER_ACTION_STATE.PLACING_BUILDING:

                if (Input.GetMouseButtonUp(1))
                {
                    CancelPlaceBuildingState();
                }

                break;

            case PLAYER_ACTION_STATE.SETTING_RALLY_POINT:

                UpdateSelectableController();

                if (Input.GetMouseButtonDown(0))
                {
                    SetNewRallyPoint();
                }

                else if (Input.GetMouseButtonUp(1))
                {
                    CancelRallyPointState();
                }

                break;

            default:
                break;
        }
    }

    public void SetNewRallyPoint()
    {
        Vector3 newRallyPointPosition = Vector3.zero;
        Building selectedBuilding = _controllerSelecting.selectedController.GetComponent<Building>();

        // Reset as default
        selectedBuilding.rallyToControllerTitle = null;
        selectedBuilding.rallyToController = null;

        if (selectableController != null)
        {
            newRallyPointPosition = selectableController.GetPrimaryNode().worldPosition + new Vector2(0.0f, -0.04f);

            // Remember this for Villagers who are rallied to resources
            selectedBuilding.rallyToController = selectableController.GetComponent<BaseController>();

            if(selectableController.controllerType == CONTROLLER_TYPE.RESOURCE)
            {
                selectedBuilding.rallyToControllerTitle = selectableController.GetComponent<BaseController>().title;
            }
        }

        else
        {
            Tile tile = Grid.instance.GetTileFromWorldPoint(mousePosition);

            // Clicked outside map, so don't do anything
            if(tile == null)
            {
                return;
            }

            else
            {
                newRallyPointPosition = Grid.instance.GetTileFromWorldPoint(mousePosition).worldPosition;
            }
        }

        selectedBuilding.rallyPointPos = newRallyPointPosition;
        rallyPointSprite.GetComponent<Transform>().position = newRallyPointPosition;
        rallyPointSprite.GetComponent<Bounce>().Action(); // Animation

        _controllerSelecting.unsafeToSelectTile = true;

        // No longer setting rally point
        CancelRallyPointState();
    }

    void UpdateSelectableController()
    {
        BaseController newSelectableController = SetMouseHoveringController();

        if (newSelectableController == null)
        {
            // Look for static Controller that occupies Tile first
            newSelectableController = Grid.instance.GetControllerFromWorldPoint(mousePosition);

            // If not found, see if any units occupy nodes in Tiles
            if (newSelectableController == null)
            {
                newSelectableController = Grid.instance.GetUnitFromWorldPoint(mousePosition);
            }
        }

        // Only change mouse cursor when selectable controller changes
        if (newSelectableController != selectableController)
        {
            if(newSelectableController != null
                && newSelectableController.IsVisible()
                && !newSelectableController.dead)
            {
                selectableController = newSelectableController;
            }

            else
            {
                selectableController = null;
            }
        }

        // No Controllers selected, so see if we can select a Tile instead and show info about it
        if(selectableController == null 
            && !CursorHoveringUI.value
            && !_controllerSelecting.attemptedToSelectMultipleUnits)
        {
            selectedTile = Grid.instance.GetTileFromWorldPoint(mousePosition);

            if (selectedTile != null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    // If we clicked on a Tile, we are no longer previously selected any controllers
                    if (_controllerSelecting.unsafeToSelectTile)
                    {
                        _controllerSelecting.unsafeToSelectTile = false;
                    }

                    // If we didn't previously select any controllers, it is now safe to select a Tile
                    else
                    {
                        Grid.instance.selectedTilePrefab.GetComponent<SpriteRenderer>().enabled = true;
                        Grid.instance.selectedTilePrefab.GetComponent<Transform>().position = selectedTile.worldPosition;

                        ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.TILE, null);
                    }
                }
            }
        }
    }

    void UpdateMouseCursor()
    {
        // Don't select invisible controllers
        if (selectableController != null && selectableController._spriteRenderer.enabled)
        {
            if (selectableController.controllerType == CONTROLLER_TYPE.RESOURCE)
            {
                HandleMouseOverResource();
            }

            else if (selectableController.controllerType == CONTROLLER_TYPE.BUILDING)
            {
                HandleMouseOverBuilding();
            }

            else if(selectableController.controllerType == CONTROLLER_TYPE.UNIT)
            {
                HandleMouseOverUnit();
            }
        }

        else
        {
            EventManager.TriggerEvent("SetDefaultCursor");
        }
    }

    void HandleMouseOverResource()
    {
        // A Villager is selected
        if (_controllerSelecting.GetSelectedGatherers().Count > 0
            && _controllerSelecting.selectedEnemy == null
            && !CursorHoveringUI.value)
        {
            Resource resource = selectableController.GetComponent<Resource>();

            switch (resource.harvestType)
            {
                case HARVEST_TYPE.CHOP: EventManager.TriggerEvent("SetChopCursor"); break;
                case HARVEST_TYPE.MINE: EventManager.TriggerEvent("SetMineCursor"); break;
                case HARVEST_TYPE.GATHER_BERRIES: EventManager.TriggerEvent("SetGatherCursor"); break;
                case HARVEST_TYPE.FARM: EventManager.TriggerEvent("SetFarmCursor"); break;
                default: EventManager.TriggerEvent("SetDefaultCursor"); break;
            }
        }

        else
        {
            EventManager.TriggerEvent("SetDefaultCursor");
        }
    }

    void HandleMouseOverBuilding()
    {
        Building building = selectableController.GetComponent<Building>();

        // Player building
        if (building.playerID == PlayerManager.myPlayerID)
        {
            if (!CursorHoveringUI.value)
            {
                if (!building.constructed && _controllerSelecting.GetSelectedGatherers().Count > 0)
                {
                    // Don;t show any Build Cursor over Civilization Center,
                    // since it's only placed and instantly constructed by Tribe unit
                    if (!building._buildingStats.isCivilizationCenter)
                    {
                        EventManager.TriggerEvent("SetBuildCursor");
                    }
                }

                // Repair
                else if (building.hitpointsLeft < building.maxHitPoints && _controllerSelecting.GetSelectedGatherers().Count > 0)
                {
                    EventManager.TriggerEvent("SetBuildCursor");
                }

                else if (building._buildingStats.resourceDeliveryPoint && _controllerSelecting.IsAnySelectedGatherersCarryingResources())
                {
                    EventManager.TriggerEvent("SetGatherCursor");
                }

                // Holding over available farm (max 1 Villager per farm)
                else if(_controllerSelecting.GetSelectedGatherers().Count > 0 
                    && building.title.Equals("Farm") 
                    && !building.GetComponent<Farm>().hasFarmer)
                {
                    EventManager.TriggerEvent("SetGatherCursor");
                }

                else
                {
                    EventManager.TriggerEvent("SetDefaultCursor");
                }
            }

            else
            {
                EventManager.TriggerEvent("SetDefaultCursor");
            }
        }

        // Enemy building
        else if (_controllerSelecting.SelectedUnitWhoCanAttack())
        {
            EventManager.TriggerEvent("SetAttackCursor");
        }

        else
        {
            EventManager.TriggerEvent("SetDefaultCursor");
        }
    }

    void HandleMouseOverUnit()
    {
        if (_controllerSelecting.GetSelectedUnits().Count > 0
            && selectableController.playerID != PlayerManager.myPlayerID
            && _controllerSelecting.SelectedUnitWhoCanAttack())
        {
            EventManager.TriggerEvent("SetAttackCursor");
        }

        else
        {
            EventManager.TriggerEvent("SetDefaultCursor");
        }
    }

    public void MoveSelectedUnitsToNewTarget(bool attackMode)
    {
        // Ignore clicks outside Grid
        if(Grid.instance.GetNodeFromWorldPoint(mousePosition) == null)
        {
            return;
        }

        List<UnitStateController> selectedUnits = _controllerSelecting.GetSelectedUnits();

        // No units selected,
        // or enemy unit selected
        if (selectedUnits.Count == 0 || selectedUnits[0].playerID != PlayerManager.myPlayerID)
        {
            return;
        }

        float averagePositionX = 0;
        float averagePositionY = 0;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            averagePositionX += selectedUnits[i]._transform.position.x;
            averagePositionY += selectedUnits[i]._transform.position.y;
        }

        averagePositionX /= selectedUnits.Count;
        averagePositionY /= selectedUnits.Count;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            // Reset any farm ownerships
            if(selectedUnits[i].farm)
            {
                selectedUnits[i].farm.hasFarmer = false;
                selectedUnits[i].farm = null;
            }

            if(attackMode && selectedUnits[i]._unitStats.canAttack)
            {
                selectedUnits[i].MoveToInAttackMode(mousePosition);
            }

            // Move to controller on location
            else if(selectableController != null)
            {
                // We are moving to a Controller that we can stand on
                if(selectableController._basicStats.walkable)
                {
                    // Special cases for walkable Controller if selected unit is a Villager
                    if (selectedUnits[i]._unitStats.isVillager)
                    {
                        // Farms have special cases
                        if(selectableController.title.Equals("Farm"))
                        {
                            // Become the new farmer
                            if(!selectableController.GetComponent<Farm>().hasFarmer)
                            {
                                // Set farmer
                                Farm farm = selectableController.GetComponent<Farm>();
                                selectedUnits[i].farm = farm;
                                farm.hasFarmer = true;

                                selectedUnits[i].MoveTo(selectableController);
                            }

                            // Move to empty location on top of farm
                            else
                            {
                                MoveToEmptyLocation(selectedUnits[i], new Vector2(averagePositionX, averagePositionY));
                            }  
                        }

                        // Move to empty location on top of walkable Controller
                        else
                        {
                            MoveToEmptyLocation(selectedUnits[i], new Vector2(averagePositionX, averagePositionY));
                        }
                    }

                    // Move to empty location on top of farm
                    else
                    {
                        MoveToEmptyLocation(selectedUnits[i], new Vector2(averagePositionX, averagePositionY));
                        continue;
                    }
                }

                // For all unwalkable buildings, move to border of Controller
                else
                {
                    selectedUnits[i].MoveTo(selectableController);
                }

                // If clicked on resource delivery point for a Villager carrying resources, 
                // we don't want the Villager to go back to resource after delivering, but stay at Building.
                if(selectedUnits[i]._unitStats.isVillager)
                {
                    selectedUnits[i].lastResouceGathered = null;

                    if(selectableController.controllerType == CONTROLLER_TYPE.RESOURCE)
                    {
                        selectedUnits[i].harvestingResource = true;
                        
                        // Remember so we can go back to continue harvesting after derlivering
                        // resources to a delivery point, or find a similair resource if it gets depleted by the time we reach it.
                        selectedUnits[i].lastResourceGatheredPosition = selectableController.GetPosition();
                        selectedUnits[i].resourceTitleCarrying = selectableController.title;
                    }
                    
                    else
                    {
                        selectedUnits[i].harvestingResource = false;

                        // Show that we are moving to the controller's loaction
                        EventManager.TriggerEvent("ActivateMoveUnitsIndicator");
                    }
                }
            }

            // Move to empty location
            else
            {
                MoveToEmptyLocation(selectedUnits[i], new Vector2(averagePositionX, averagePositionY));
            }
        }
    }

    bool IsBuildingBeingConstructed()
    {
        return selectableController.controllerType == CONTROLLER_TYPE.BUILDING && !selectableController.GetComponent<Building>().constructed;
    }

    void MoveToEmptyLocation(UnitStateController controller, Vector2 averagePosition)
    {
        Vector2 offset = new Vector2(averagePosition.x - controller._transform.position.x, averagePosition.y - controller._transform.position.y);

        if (Grid.instance.GetNodeFromWorldPoint(mousePosition - offset) != null)
        {
            controller.MoveTo(mousePosition - offset);
        }

        else
        {
            controller.MoveTo(mousePosition);
        }

        // Show which location we are moving to
        EventManager.TriggerEvent("ActivateMoveUnitsIndicator");
    }

    private BaseController SetMouseHoveringController()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
        
        // Reset
        mouseHoveringController.controller = null;
        mouseHoveringController.priorityValue = -1;

        // Iterate through all hit objects and see which one we should set as hovering to select
        for(int i = 0; i < hits.Length; i++)
        {
            BaseController controller = hits[i].collider.gameObject.GetComponent<BaseController>();
            int priority = GetHoveringPriorityOfSelectedController(controller);

            if (priority == mouseHoveringController.priorityValue
                && (controller.zIndex < mouseHoveringController.controller.zIndex))
            {
                mouseHoveringController.controller = controller;
                mouseHoveringController.priorityValue = priority;
            }

            else if(priority > mouseHoveringController.priorityValue)
            {
                mouseHoveringController.controller = controller;
                mouseHoveringController.priorityValue = priority;
            }
        }

        return mouseHoveringController.controller;
    }

    int GetHoveringPriorityOfSelectedController(BaseController controller)
    {
        // Set priority
        if (_controllerSelecting.SelectedUnitWhoCanAttack() && controller.playerID != myPlayerID)
        {
            return 2;
        }

        else if (controller.controllerType == CONTROLLER_TYPE.RESOURCE)
        {
            return 1;
        }

        else
        {
            return 1;
        }
    }

    public void StopAction()
    {
        if (_controllerSelecting.selectedController != null)
        {
            if(_controllerSelecting.selectedController.controllerType == CONTROLLER_TYPE.BUILDING)
            {
                _controllerSelecting.selectedController.Cancel();
            }
        }

        List<UnitStateController> selectedUnits = _controllerSelecting.GetSelectedUnits();

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            selectedUnits[i].Cancel();
        }
    }

    public void AddFriendlyUnitReference(UnitStateController unit, int player)
    {
        friendlyUnits.Add(unit);
    }

    public void RemoveFriendlyUnitReference(UnitStateController unit, int player)
    {
        friendlyUnits.Remove(unit);
    }

    public List<UnitStateController> GetAllFriendlyUnits()
    {
        return friendlyUnits;
    }

    void UpdateFoodIntake()
    {
        for(int i = 0; i < WorldManager.instance.numPlayers; i++)
        {
            PlayerDataManager.instance.playerData[i].foodStock += (Time.deltaTime * PlayerDataManager.instance.playerData[i].foodIntake) / timePerFoodIntakePoint;
        }

        timeSinceLastFoodUIUpdate += Time.deltaTime;
        if(timeSinceLastFoodUIUpdate >= timeBetweenFoodUIUpdates)
        {
            EventManager.TriggerEvent("UpdateFoodStockUI");
            EventManager.TriggerEvent("UpdateProsperityStockUI");
        }
    }

    void UpdateNewCitizens()
    {
        for (int i = 0; i < WorldManager.instance.numPlayers; i++)
        {
            PlayerData data = PlayerDataManager.instance.playerData[i];

            float spawnCitizenFactor = data.realProsperity;

            if (spawnCitizenFactor < -4)
            {
                return;
            }

            if (spawnCitizenFactor < 0)
            {
                spawnCitizenFactor *= 10;
            }

            data.progressTowardsNewCitizen += (Time.deltaTime / (20 - spawnCitizenFactor * 1.0f));

            if (data.progressTowardsNewCitizen >= 1.0f)
            {
                data.progressTowardsNewCitizen = 0.0f + (data.progressTowardsNewCitizen - 1.0f);
                data.newCitizens++;
                EventManager.TriggerEvent("UpdateNewCitizensStockUI");
            }
        }
    }
}
