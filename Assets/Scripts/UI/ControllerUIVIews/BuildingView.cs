﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Show info about selected building
[CreateAssetMenu(menuName = "UI/Controller views/building")]
public class BuildingView : ControllerUIView
{
    Building buildingController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        buildingController = controller.GetComponent<Building>();

        ui.ShowHitpoints(buildingController.hitpointsLeft, buildingController.maxHitPoints);

        if (PlayerManager.myPlayerID == _controller.playerID)
        {
            ui.ShowStats(buildingController.statSprites, buildingController.GetUniqueStats());
            ui.ShowProductionButtons(buildingController.productionButtonsData);
        }

        EventManager.TriggerEvent("ActivateProductionButtonsView");
    }

    public override void Update()
    {
        // TODO DON'T HIDE IF OPENING PRODUCTiON VIEW
        ui.UpdateHitpoints(buildingController.hitpointsLeft, buildingController.maxHitPoints);
    }

    public override void OnExit()
    {
        ui.HideStats();
        ui.HideHitpoints();

        // TODO DON'T HIDE IF OPENING PRODUCTiON VIEW
        ui.HideProductionButtons();

        EventManager.TriggerEvent("DisableProductionButtonsView");
    }
}
