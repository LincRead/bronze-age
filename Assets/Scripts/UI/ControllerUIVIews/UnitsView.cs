using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsView : ControllerUIView
{
    int numButtonsActivated = 0;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        ShowSelectedUnits();
        EventManager.TriggerEvent("ActivateUnitActionsView");
    }

    void ShowSelectedUnits()
    {
        // Show selected units
        List<UnitStateController> selectedUnits = PlayerManager.instance._controllerSelecting.selectedUnits;
        for (int i = 0; i < selectedUnits.Count && i < ControllerSelecting.maxUnitsSelected; i++)
        {
            ui._selectedUnitButtons[i].UpdateButton(selectedUnits[i]);
            numButtonsActivated++;
        }
    }

    public override void Update()
    {
        for (int i = 0; i < numButtonsActivated; i++)
        {
            ui._selectedUnitButtons[i].UpdateHealth();
        }
    }

    public override void OnExit()
    {
        numButtonsActivated = 0;
        ui.HideSelectedUnitsButtons();
        EventManager.TriggerEvent("DisableUnitActionsView");
    }
}
