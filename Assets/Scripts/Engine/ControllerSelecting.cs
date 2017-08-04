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

    Building selectedBuilding = null;
    Resource selectedResource = null;

    private bool mouseIsHoveringGUI;

    void Update()
    {
        mouseIsHoveringGUI = PlayerManager.instance._cursorHoveringUI.IsCursorHoveringUI();

        // Don't select anything unless player is in default state
        if (PlayerManager.instance.currentUserState == PlayerManager.PLAYER_ACTION_STATE.DEFAULT
            && !mouseIsHoveringGUI)
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
        CreateSelectionRectangle();

        if (Vector2.Distance(mousePosScreenToWorldPointStart, mousePosScreenToWorldPointEnd) < .1f)
        {
            FindControllerToSelect();
        }

        else
        {
            SelectUnits(selectionRect);
        }

        showSelectBox = false;
    }

    void CreateSelectionRectangle()
    {
        mousePosScreenToWorldPointEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        selectionRect = new Rect(
            mousePosScreenToWorldPointStart.x,
            mousePosScreenToWorldPointStart.y,
            mousePosScreenToWorldPointEnd.x - mousePosScreenToWorldPointStart.x,
            mousePosScreenToWorldPointEnd.y - mousePosScreenToWorldPointStart.y);
    }

    void FindControllerToSelect()
    {
        if (!FindAndSelectObject(mousePosScreenToWorldPointEnd))
        {
            SelectUnit(selectionRect);
        }

        // Selected building
        else if (selectedBuilding != null)
        {
            if (!selectedBuilding.constructed)
                UnitUIManager.instance.ShowConstructionProgress();
            else
                UnitUIManager.instance.ShowBuildingUI(selectedBuilding);

            DeselectAllFriendlyUnits();
        }

        else if (selectedResource != null)
        {
            // Todo show resource UI
            UnitUIManager.instance.ShowResourceUI(selectedResource);

            DeselectAllFriendlyUnits();
        }
    }

    void OnGUI()
    {
        // Show selection box
        if(showSelectBox && !mouseIsHoveringGUI)
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

    public void SelectUnit(Rect selectionBox)
    {
        selectedGatherers.Clear();

        if (SetUnitAsSelected(selectionBox))
            UnitUIManager.instance.ShowVillagerUI(selectedGatherers[0]);
        else
            UnitUIManager.instance.ShowDefaultUI();
    }

    public void SelectUnits(Rect selectionBox)
    {
        selectedGatherers.Clear();

        if (SetUnitsAsSelected(selectionBox))
            UnitUIManager.instance.ShowVillagerUI(selectedGatherers[0]);
        else
            UnitUIManager.instance.ShowDefaultUI();
    }

    // Returns true if selected a villager
    bool SetUnitAsSelected(Rect collisionBox)
    {
        List<UnitStateController> friendlyUnits = PlayerManager.instance.GetAllFriendlyUnits();
        bool selectedVillager = false;

        for (int i = 0; i < friendlyUnits.Count; i++)
        {
            if (!selectedVillager && friendlyUnits[i].IntersectsRectangle(collisionBox))
            {
                friendlyUnits[i].Select();
                if (friendlyUnits[i]._unitStats.gatherer)
                {
                    selectedGatherers.Add(friendlyUnits[i]);
                    selectedVillager = true;
                }
            }
            else
                friendlyUnits[i].Deselect();
        }

        return selectedVillager;
    }

    bool FindAndSelectObject(Vector2 mousePos)
    {
        if(PlayerManager.instance.selectableResource != null)
        {
            SelectResource(PlayerManager.instance.selectableResource);
            return true;
        }

        Tile nodeAtMousePos = Grid.instance.GetTileFromWorldPoint(mousePos);

        if (nodeAtMousePos == null)
            return false;

        if (nodeAtMousePos.buildingOccupying)
        {
            SelectBuilding(nodeAtMousePos.buildingOccupying);
            return true;
        }

        return false;
    }

    public bool SetUnitsAsSelected(Rect collisionBox)
    {
        List<UnitStateController> friendlyUnits = PlayerManager.instance.GetAllFriendlyUnits();
        bool selectedGatherer = false;

        for (int i = 0; i < friendlyUnits.Count; i++)
        {
            if (friendlyUnits[i].IntersectsRectangle(collisionBox))
            {
                friendlyUnits[i].Select();
                if (friendlyUnits[i]._unitStats.gatherer)
                {
                    selectedGatherers.Add(friendlyUnits[i]);
                    selectedGatherer = true;
                }
            }
            else
                friendlyUnits[i].Deselect();
        }

        return selectedGatherer;
    }

    public void SelectBuilding(Building building)
    {
        ResetSelection();
        building.Select();
        selectedBuilding = building;
    }

    public void SelectResource(Resource resource)
    {
        ResetSelection();
        resource.Select();
        selectedResource = resource;
    }

    void ResetSelection()
    {
        if (selectedBuilding != null)
        {
            selectedBuilding.Deselect();
            selectedBuilding = null;
        }

        if (selectedResource != null)
        {
            selectedResource.Deselect();
            selectedResource = null;
        }
    }

    public List<UnitStateController> GetSelectedGatherers()
    {
        return selectedGatherers;
    }

    public void DeselectAllFriendlyUnits()
    {
        for(int i = 0; i < selectedGatherers.Count; i++)
        {
            selectedGatherers[i].Deselect();
        }

        selectedGatherers.Clear();
    }
}
