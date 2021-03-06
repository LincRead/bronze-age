﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsView : ControllerUIView
{
    int numButtonsActivated = 0;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        // Reset
        ui.HideSelectedUnitsButtons();

        // Show selected
        ShowSelectedUnits();
    }

    void ShowSelectedUnits()
    {
        bool onlyVillagersSelected = true;

        // Show selected units
        List<UnitStateController> selectedUnits = PlayerManager.instance._controllerSelecting.selectedUnits;
        for (int i = 0; i < selectedUnits.Count && i < ControllerSelecting.maxUnitsSelected; i++)
        {
            ui._selectedUnitButtons[i].UpdateButton(selectedUnits[i]);
            numButtonsActivated++;

            if (!selectedUnits[i]._unitStats.isVillager)
            {
                onlyVillagersSelected = false;
            }
        }

        if(onlyVillagersSelected)
        {
            EventManager.TriggerEvent("ActivateVillagerView");
        }

        EventManager.TriggerEvent("ActivateUnitActionsView");
    }

    public override void Update()
    {
        for (int i = 0; i < numButtonsActivated; i++)
        {
            if(ui._selectedUnitButtons[i] != null)
            {
                ui._selectedUnitButtons[i].UpdateHealth();
            }
        }
    }

    public override void OnExit()
    {
        numButtonsActivated = 0;
        ui.HideSelectedUnitsButtons();
        EventManager.TriggerEvent("DisableVillagerView");
        EventManager.TriggerEvent("DisableUnitActionsView");
        EventManager.TriggerEvent("DisableBuildingsView");
    }
}
