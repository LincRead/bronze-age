﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PlayerManager : MonoBehaviour {

    [HideInInspector]
    public static int myPlayerID = 0;

    public enum PLAYER_ACTION_STATE
    {
        DEFAULT,
        ATTACK_MOVE,
        PLACING_BUILDING,
        MENU
    }

    [HideInInspector]
    public PLAYER_ACTION_STATE currentUserState = PLAYER_ACTION_STATE.DEFAULT;

    [HideInInspector]
    public ControllerSelecting _controllerSelecting;

    [HideInInspector]
    public List<UnitStateController> friendlyUnits = new List<UnitStateController>();

    [HideInInspector]
    public Building buildingBeingPlaced = null;

    [HideInInspector]
    public BaseController mouseHoveringController = null;

    [HideInInspector]
    public BaseController selectableController = null;

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
    }

    public void SetBuildingPlacementState(Building building)
    {
        buildingBeingPlaced = building;

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
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch (currentUserState)
        {
            case PLAYER_ACTION_STATE.DEFAULT:

                UpdateSelectableController();

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

            default:
                break;
        }
    }

    void UpdateSelectableController()
    {
        // Look for static Controller that occupies Tile first
        BaseController newSelectableController = Grid.instance.GetControllerFromWorldPoint(mousePosition);

        // If not found, see if any units occupy nodes in Tiles
        if (newSelectableController == null)
        {
            newSelectableController = Grid.instance.GetUnitFromWorldPoint(mousePosition);
        }
            
        if (mouseHoveringController != null)
        {
            newSelectableController = mouseHoveringController;
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
            
            UpdateMouseCursor();
        }
    }

    void UpdateMouseCursor()
    {
        // Don't select invisible controllers
        if (selectableController != null && selectableController._spriteRenderer.enabled)
        {
            if (selectableController.controllerType == CONTROLLER_TYPE.STATIC_RESOURCE)
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
        if (_controllerSelecting.GetSelectedGatherers().Count > 0
            && _controllerSelecting.selectedEnemy == null // Not selected an enemy Villager
            && !CursorHoveringUI.value)
        {
            Resource resource = selectableController.GetComponent<Resource>();

            switch (resource.resourceType)
            {
                case HARVEST_TYPE.CHOP: EventManager.TriggerEvent("SetChopCursor"); break;
                case HARVEST_TYPE.MINE: EventManager.TriggerEvent("SetMineCursor"); break;
                case HARVEST_TYPE.GATHER: EventManager.TriggerEvent("SetGatherCursor"); break;
                case HARVEST_TYPE.FARM: EventManager.TriggerEvent("SetFarmCursor"); break;
                default: EventManager.TriggerEvent("SetDefaultCursor"); break;
            }
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
                // Construct
                if (!building.constructed)
                {
                    EventManager.TriggerEvent("SetBuildCursor");
                }

                // Repair
                else if (building.hitpointsLeft < building.maxHitPoints)
                {
                    EventManager.TriggerEvent("SetBuildCursor");
                }
            }

            else
            {
                EventManager.TriggerEvent("SetDefaultCursor");
            }
        }

        // Enemy building
        else if (_controllerSelecting.GetSelectedUnits().Count > 0)
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
            && selectableController.playerID != PlayerManager.myPlayerID)
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
            if(attackMode)
            {
                selectedUnits[i].MoveToInAttackMode(mousePosition);
            }

            // Move to controller on location
            else if(selectableController != null)
            {
                selectedUnits[i].MoveTo(selectableController);
            }

            // Move to empty location
            else
            {
                Vector2 offset = new Vector2(averagePositionX - selectedUnits[i]._transform.position.x, averagePositionY - selectedUnits[i]._transform.position.y);
                selectedUnits[i].MoveTo(mousePosition - offset);
            }
        }

        EventManager.TriggerEvent("ActivateMoveUnitsIndicator");
    }

    public void StopAction()
    {
        List<UnitStateController> selectedUnits = _controllerSelecting.GetSelectedUnits();

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            selectedUnits[i].TransitionToState(selectedUnits[i].idleState);
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
}
