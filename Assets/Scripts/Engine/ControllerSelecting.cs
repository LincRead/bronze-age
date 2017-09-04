using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerSelecting : MonoBehaviour {

    public static int maxUnitsSelected = 18;

    Vector3 mousePosStart;
    Vector3 mousePosScreenToWorldPointStart;
    Vector3 mousePosScreenToWorldPointEnd;
    Rect selectionRect;

    [HideInInspector]
    public bool showSelectBox = false;

    private List<UnitStateController> selectedGatherers = new List<UnitStateController>();

    [HideInInspector]
    public List<UnitStateController> selectedUnits = new List<UnitStateController>();

    [HideInInspector]
    public UnitStateController selectedEnemy = null;

    [HideInInspector]
    public BaseController selectedController = null;

    protected float timeSinceLastSingleUnitSelected = 0.0f;
    protected float doubleClickUnitTime = 1.0f;
    protected UnitStateController lastSelectedUnit;
    protected float maxDistanceToFindUnitsOfSameType = 300;

    void Update()
    {
        // Don't select anything unless player is in default state
        if (PlayerManager.instance.currentUserState == PlayerManager.PLAYER_ACTION_STATE.DEFAULT
            && !CursorHoveringUI.value)
        {
            UpdateSelecting();
        }
    }

    void UpdateSelecting()
    {
        timeSinceLastSingleUnitSelected += Time.deltaTime;

        // If we press the left mouse button, save mouse location and begin controller selection
        if (Input.GetMouseButtonDown(0))
        {
            InitialSelecting();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ExecuteSelecting();
        }
    }

    void InitialSelecting()
    {
        mousePosStart = Input.mousePosition;

        // Move origin from bottom left to top left
        mousePosStart.y = Screen.height - mousePosStart.y;

        mousePosScreenToWorldPointStart = PlayerManager.mousePosition;

        showSelectBox = true;
    }

    void ExecuteSelecting()
    {
        ResetSelection();
        mousePosScreenToWorldPointEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CreateSelectionRectangle();

        if (Vector2.Distance(mousePosScreenToWorldPointStart, mousePosScreenToWorldPointEnd) < .1f)
        {
            SelectController();
        }

        else
        {
            FindUnitsToSelect(selectionRect);
        }

        showSelectBox = false;
    }

    void CreateSelectionRectangle()
    {
        selectionRect = new Rect(
            mousePosScreenToWorldPointStart.x,
            mousePosScreenToWorldPointStart.y,
            mousePosScreenToWorldPointEnd.x - mousePosScreenToWorldPointStart.x,
            mousePosScreenToWorldPointEnd.y - mousePosScreenToWorldPointStart.y);
    }

    void SelectController()
    {
        // Select unit if Tile is not occupied by a controller
        if (!FindAndSelectControllerOccupyingTile(mousePosScreenToWorldPointEnd))
        {
            SelectUnit(selectionRect);
        }

        else
        {
            if (selectedController.controllerType == CONTROLLER_TYPE.BUILDING)
            {
                SelectBuilding();
            }

            if(selectedController.controllerType == CONTROLLER_TYPE.STATIC_RESOURCE)
            {
                SelectResource();
            }
        }
    }

    void SelectUnit(Rect selectionBox)
    {
        SetUnitAsSelected();
        ChangeToUnitView();
    }

    void FindUnitsToSelect(Rect selectionBox)
    {
        SetUnitsAsSelected(selectionBox);
        ChangeToUnitView();
    }

    void FindUnitsOfSameTypeToSelect(UnitStateController unit)
    {
        SetUnitsOfSameTypeAsSelected(unit);
        ChangeToUnitView();
    }

    void ChangeToUnitView()
    {
        if (selectedUnits.Count > 1)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.SELECTED_UNITS, null);
        }

        else if(selectedUnits.Count > 0)
        {
            if(selectedUnits[0]._unitStats.isVillager)
            {
                ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.VILLAGER, selectedUnits[0]);
            }

            else if(!selectedUnits[0]._unitStats.canAttack)
            {
                if(selectedUnits[0].title.Equals("Tribe"))
                {
                    ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.TRIBE, selectedUnits[0]);
                }

                // Other units who can't attack
            }

            else
            {
                ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.WARRIOR, selectedUnits[0]);
            }
        }

        else
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);
        }
    }

    void SelectBuilding()
    {
        Building selectedBuilding = selectedController.GetComponent<Building>();

        if (selectedBuilding.producing)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.PRODUCTION_PROGRESS, selectedBuilding);
        }

        else if (selectedBuilding.constructed)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDING_INFO, selectedBuilding);
        }

        // Show construction progress for all other buildings being constructed
        else
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.CONSTRUCTION_PROGRESS, selectedBuilding);
        }
    }

    void SelectResource()
    {
        ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.RESOURCE_INFO, selectedController);
    }

    bool FindAndSelectControllerOccupyingTile(Vector2 mousePos)
    {
        BaseController controller = PlayerManager.instance.selectableController;

        if (controller != null && controller.controllerType != CONTROLLER_TYPE.UNIT)
        {
            controller.Select();
            selectedController = controller;
            return true;
        }

        return false;
    }

    public void SetControllerAsSelected(BaseController controller)
    {
        controller.Select();
        selectedController = controller;
    }

    public void SetUnitAsSelected()
    {
        if(PlayerManager.instance.selectableController != null
            && PlayerManager.instance.selectableController._spriteRenderer.enabled
            && PlayerManager.instance.selectableController.controllerType == CONTROLLER_TYPE.UNIT
            && !PlayerManager.instance.selectableController.dead)
        {
            UnitStateController unit = PlayerManager.instance.selectableController.GetComponent<UnitStateController>();

            if(unit._unitStats.isTribe && unit.GetComponent<TribeController>().movingTowarsCamp)
            {
                return;
            }

            // Double clicked unit
            if (timeSinceLastSingleUnitSelected <= doubleClickUnitTime
                && unit == lastSelectedUnit)
            {
                FindUnitsOfSameTypeToSelect(unit);
            }

            else
            {
                unit.Select();
                lastSelectedUnit = unit;

                // Store unit selected
                if (unit.playerID == PlayerManager.myPlayerID)
                {
                    selectedUnits.Add(unit);
                }

                else
                {
                    selectedEnemy = unit;
                }

                if (unit._unitStats.isVillager)
                {
                    selectedGatherers.Add(unit);
                }
            }

            timeSinceLastSingleUnitSelected = 0.0f;
        }
    }

    void SetUnitsAsSelected(Rect collisionBox)
    {
        List<UnitStateController> friendlyUnits = PlayerManager.instance.GetAllFriendlyUnits();

        // Can select max 18 units atm
        for (int i = 0; i < friendlyUnits.Count && i < maxUnitsSelected; i++)
        {
            if (!friendlyUnits[i].dead && friendlyUnits[i].IntersectsRectangle(collisionBox))
            {
                friendlyUnits[i].Select();

                selectedUnits.Add(friendlyUnits[i]);

                if (friendlyUnits[i]._unitStats.isVillager)
                {
                    selectedGatherers.Add(friendlyUnits[i]);
                }
            }
        }
    }

    void SetUnitsOfSameTypeAsSelected(UnitStateController unit)
    {
        List<UnitStateController> friendlyUnits = PlayerManager.instance.GetAllFriendlyUnits();

        // Can select max 18 units atm
        for (int i = 0; i < friendlyUnits.Count && i < maxUnitsSelected; i++)
        {
            if (friendlyUnits[i] != null
                && !friendlyUnits[i].dead
                && unit.title.Equals(friendlyUnits[i].title)
                && Grid.instance.GetDistanceBetweenNodes(unit.GetPrimaryNode(), friendlyUnits[i].GetPrimaryNode()) <= maxDistanceToFindUnitsOfSameType)
            {
                friendlyUnits[i].Select();

                selectedUnits.Add(friendlyUnits[i]);

                if (friendlyUnits[i]._unitStats.isVillager)
                {
                    selectedGatherers.Add(friendlyUnits[i]);
                }
            }
        }
    }

    public void ResetSelection()
    {
        DeselectAllFriendlyUnits();

        if (selectedEnemy != null)
        {
            selectedEnemy.Deselect();
            selectedEnemy = null;
        }

        if (selectedController != null)
        {
            selectedController.Deselect();
            selectedController = null;
        }
    }

    public void DeselectAllFriendlyUnits()
    {
        for(int i = 0; i < selectedUnits.Count; i++)
        {
            selectedUnits[i].Deselect();
        }

        selectedUnits.Clear();
        selectedGatherers.Clear();
    }

    void OnGUI()
    {
        // Show selection box
        if(showSelectBox && !CursorHoveringUI.value)
        {
            Vector2 mousePosEnd = Input.mousePosition;

            // Move origin from bottom left to top left
            mousePosEnd.y = Screen.height - mousePosEnd.y;

            selectionRect = new Rect(
                mousePosStart.x,
                mousePosStart.y,
                mousePosEnd.x - mousePosStart.x,
                mousePosEnd.y - mousePosStart.y);

            Utils.DrawScreenRect(selectionRect, new Color(0.15f, 0.9f, 0.15f, 0.2f));
            Utils.DrawScreenRectBorder(selectionRect, 1, new Color(0.15f, 0.9f, 0.15f));
        }
    }

    public void RemoveFromSelectedUnits(UnitStateController unit)
    {
        selectedUnits.Remove(unit);

        if (selectedGatherers.Contains(unit))
        {
            selectedGatherers.Remove(unit);
        }

        if (selectedUnits.Count == 0)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);
        }

        // Reset Units View
        else
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.SELECTED_UNITS, null);
        }
    }

    public bool SelectedUnitWhoCanAttack()
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (selectedUnits[i]._unitStats.canAttack)
            {
                return true;
            }
        }

        return false;
    }

    public List<UnitStateController> GetSelectedGatherers()
    {
        return selectedGatherers;
    }

    public List<UnitStateController> GetSelectedUnits()
    {
        return selectedUnits;
    }
}
