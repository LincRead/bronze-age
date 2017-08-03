using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSelection : MonoBehaviour {

    Vector3 mousePosInitial;
    Vector3 mousePostToWorldPointInitial;
    Rect selectionRect;

    [HideInInspector]
    public bool isSelecting = false;

    List<UnitStateController> selectedGatherers = new List<UnitStateController>();
    Building selectedBuilding = null;
    Resource selectedResource = null;

    void Update()
    {
        if (WorldManager.instance.currentUserState != WorldManager.USER_STATE.NONE)
            return;

        // If we press the left mouse button, save mouse location and begin selection
        if (Input.GetMouseButtonDown(0) && !WorldManager.instance._cursorHoveringUI.IsCursorHoveringUI())
        {
            mousePosInitial = Input.mousePosition;

            // Move origin from bottom left to top left
            mousePosInitial.y = Screen.height - mousePosInitial.y;

            mousePostToWorldPointInitial = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            isSelecting = true;
        }

        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0))
        {
            if(!WorldManager.instance._cursorHoveringUI.IsCursorHoveringUI() && isSelecting)
                CreateSelectionRect();

            isSelecting = false;
        } 
    }

    void CreateSelectionRect()
    {
        Vector2 mousePosEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        selectionRect = new Rect(
            mousePostToWorldPointInitial.x,
            mousePostToWorldPointInitial.y,
            mousePosEnd.x - mousePostToWorldPointInitial.x,
            mousePosEnd.y - mousePostToWorldPointInitial.y);

        ResetSelection();

        if (Vector2.Distance(mousePostToWorldPointInitial, mousePosEnd) < .1f)
        {
            if(!FindAndSelectObject(mousePosEnd))
            {
                SelectUnit(selectionRect);
            }

            // Selected building
            else if(selectedBuilding != null)
            {
                if(!selectedBuilding.constructed)
                    UnitUIManager.instance.ShowConstructionProgress(); 
                else
                    UnitUIManager.instance.ShowBuildingUI(selectedBuilding);

                DeselectAllFriendlyUnits();
            }

            else if(selectedResource != null)
            {
                // Todo show resource UI
                UnitUIManager.instance.ShowResourceUI(selectedResource);

                DeselectAllFriendlyUnits();
            }

            return;
        }

        // Move?
        SelectUnits(selectionRect);
    }

    void OnGUI()
    {
        if(isSelecting && !WorldManager.instance._cursorHoveringUI.IsCursorHoveringUI())
        {
            Vector2 mousePosEnd = Input.mousePosition;

            // Move origin from bottom left to top left
            mousePosEnd.y = Screen.height - mousePosEnd.y;

            selectionRect = new Rect(
                mousePosInitial.x,
                mousePosInitial.y,
                mousePosEnd.x - mousePosInitial.x,
                mousePosEnd.y - mousePosInitial.y);

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
        List<UnitStateController> friendlyUnits = WorldManager.instance.GetFriendlyUnits();
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
        if(WorldManager.instance.selectableResource != null)
        {
            SelectResource(WorldManager.instance.selectableResource);
            return true;
        }

        Grid grid = WorldManager.instance.GetGrid();
        Tile nodeAtMousePos = grid.GetTileFromWorldPoint(mousePos);

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
        List<UnitStateController> friendlyUnits = WorldManager.instance.GetFriendlyUnits();
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

    public Building GetSelectedBuilding()
    {
        return selectedBuilding;
    }

    public Resource GetSelecteResource()
    {
        return selectedResource;
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
