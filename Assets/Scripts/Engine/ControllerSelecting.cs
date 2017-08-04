using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerSelecting : MonoBehaviour {

    Vector3 mousePosStart;
    Vector3 mousePosScreenToWorldPointStart;
    Vector3 mousePosScreenToWorldPointEnd;
    Rect selectionRect;

    [HideInInspector]
    public bool showSelectBox = false;

    private List<UnitStateController> selectedGatherers = new List<UnitStateController>();
    private List<UnitStateController> selectedUnits = new List<UnitStateController>();

    BaseController selectedController = null;

    void Update()
    {
        // Don't select anything unless player is in default state
        if (PlayerManager.instance.currentUserState == PlayerManager.PLAYER_ACTION_STATE.DEFAULT
            && !CursorHoveringUI.value)
            UpdateSelecting();
    }

    void UpdateSelecting()
    {
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
            SelectUnits(selectionRect);
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
        // Try to select unit if tile is not occupied
        if (!FindAndSelectController(mousePosScreenToWorldPointEnd))
        {
            SelectUnit(selectionRect);
        }

        else
        {
            if (selectedController.controllerType == BaseController.CONTROLLER_TYPE.BUILDING)
            {
                Building selectedBuilding = selectedController.GetComponent<Building>();

                if (!selectedBuilding.constructed)
                    UnitUIManager.instance.ShowConstructionProgress();
                else
                    UnitUIManager.instance.ShowBuildingUI(selectedBuilding);
            }

            if(selectedController.controllerType == BaseController.CONTROLLER_TYPE.STATIC_RESOURCE)
            {
                // Todo show resource UI
                UnitUIManager.instance.ShowResourceUI(selectedController.GetComponent<Resource>());
            }
        }
    }

    void SelectUnit(Rect selectionBox)
    {
        SetUnitAsSelected(selectionBox);

        if(selectedGatherers.Count > 0)
            UnitUIManager.instance.ShowVillagerUI(selectedGatherers[0]);
        else
            UnitUIManager.instance.ShowDefaultUI();
    }

    void SelectUnits(Rect selectionBox)
    {
        SetUnitAsSelected(selectionBox);

        if(selectedGatherers.Count > 0)
            UnitUIManager.instance.ShowVillagerUI(selectedGatherers[0]);
        else
            UnitUIManager.instance.ShowDefaultUI();
    }

    bool FindAndSelectController(Vector2 mousePos)
    {
        Tile nodeAtMousePos = Grid.instance.GetTileFromWorldPoint(mousePos);

        if (nodeAtMousePos == null)
            return false;

        BaseController controller = nodeAtMousePos.controllerOccupying;

        if (controller != null)
        {
            controller.Select();
            selectedController = controller;
            return true;
        }

        return false;
    }

    void SetUnitAsSelected(Rect collisionBox)
    {
        List<UnitStateController> friendlyUnits = PlayerManager.instance.GetAllFriendlyUnits();

        for (int i = 0; i < friendlyUnits.Count; i++)
        {
            if (friendlyUnits[i].IntersectsRectangle(collisionBox))
            {
                friendlyUnits[i].Select();

                selectedUnits.Add(friendlyUnits[i]);

                if (friendlyUnits[i]._unitStats.gatherer)
                {
                    selectedGatherers.Add(friendlyUnits[i]);
                }

                return;
            }
        }
    }

    void SetUnitsAsSelected(Rect collisionBox)
    {
        List<UnitStateController> friendlyUnits = PlayerManager.instance.GetAllFriendlyUnits();

        for (int i = 0; i < friendlyUnits.Count; i++)
        {
            if (friendlyUnits[i].IntersectsRectangle(collisionBox))
            {
                friendlyUnits[i].Select();

                selectedUnits.Add(friendlyUnits[i]);

                if (friendlyUnits[i]._unitStats.gatherer)
                {
                    selectedGatherers.Add(friendlyUnits[i]);
                }
            }
        }
    }

    void ResetSelection()
    {
        DeselectAllFriendlyUnits();

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

    public List<UnitStateController> GetSelectedGatherers()
    {
        return selectedGatherers;
    }

    public List<UnitStateController> GetSelectedUnits()
    {
        return selectedUnits;
    }
}
