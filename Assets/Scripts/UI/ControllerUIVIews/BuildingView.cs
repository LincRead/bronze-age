﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Show info about selected building
[CreateAssetMenu(menuName = "UI/Controller views/building")]
public class BuildingView : ControllerUIView
{
    Building _buildingController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        _buildingController = controller.GetComponent<Building>();

        ui.healthBar.ShowHitpoints(_buildingController.hitpointsLeft, _buildingController.maxHitPoints);

        if (PlayerManager.myPlayerID == _controller.playerID)
        {
            ui.ShowProductionButtons(_buildingController.productionButtonsData);
        }

        if (_buildingController.inProductionProcess)
        {
            ShowProduction();
        }

        else
        {
            ShowBuildingStats();
        }
    }

    public void ShowBuildingStats()
    {
        if (PlayerManager.myPlayerID == _controller.playerID)
        {
            ui.ShowStats(_buildingController.statSprites, _buildingController.GetUniqueStats());
        }

        ui.productionProgressCanvas.gameObject.SetActive(false);
        ui.productionQueueCanvas.gameObject.SetActive(false);
    }

    public void ShowProduction()
    {
        ui.HideStats();

        ui.productionProgressCanvas.gameObject.SetActive(true);

        UpdatePercentProductionVisuals();

        if (_buildingController.constructed)
        {
            ui.productionQueueCanvas.gameObject.SetActive(true);
            _buildingController.UpdateProducionQueue();
        }
    }

    public override void Update()
    {
        if (_buildingController.inProductionProcess)
        {
            UpdatePercentProductionVisuals();
        }

         ui.healthBar.UpdateHitpoints(_buildingController.hitpointsLeft, _buildingController.maxHitPoints);
    }

    void UpdatePercentProductionVisuals()
    {
        float percent = _buildingController.GetPercentageProduced();
        ui.productionProgressCanvas.UpdateProgress(percent);
    }

    public override void OnExit()
    {
        ui.HideStats();
        ui.healthBar.HideHitpoints();
        ui.HideProductionButtons();

        ui.productionProgressCanvas.gameObject.SetActive(false);
        ui.productionQueueCanvas.gameObject.SetActive(false);
    }
}
