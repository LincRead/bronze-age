using System.Collections;
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

            // Change icon to next production item's
            ControllerUIManager.instance.productionProgressCanvas.icon.sprite = _buildingController.GetCurrentProduction().icon;

            UpdatePercentProductionVisuals();
        }

        else
        {
            ShowBuildingStats();
        }

        // Show Rally Point button?
        SetRallyPointButtonActive();
    }

    void SetRallyPointButtonActive()
    {
        if (_buildingController.canSetRallyPoint)
        {
            ControllerUIManager.instance.rallyPointButton.gameObject.SetActive(true);
            PlayerManager.instance.rallyPointSprite.gameObject.SetActive(true);

            // Only bounce if changed building
            if (PlayerManager.instance.rallyPointSprite.GetComponent<Transform>().position != _buildingController.rallyPointPos)
            {
                PlayerManager.instance.rallyPointSprite.GetComponent<Transform>().position = _buildingController.rallyPointPos;

                // Animate new Rally Point
                float bounceTime = 0.25f;
                LeanTween.scale(PlayerManager.instance.rallyPointSprite.gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.0f);
                LeanTween.scale(PlayerManager.instance.rallyPointSprite.gameObject, new Vector3(0.75f, 0.75f, 1.0f), bounceTime);
                LeanTween.scale(PlayerManager.instance.rallyPointSprite.gameObject, new Vector3(1.0f, 1.0f, 1.0f), bounceTime).setDelay(bounceTime);
            }
        }

        else
        {
            HideRallyPoint();
        }
    }

    void HideRallyPoint()
    {
        PlayerManager.instance.rallyPointSprite.gameObject.SetActive(false);
        ControllerUIManager.instance.rallyPointButton.gameObject.SetActive(false);
    }

    public void ShowBuildingStats()
    {
        if (PlayerManager.myPlayerID == _controller.playerID)
        {
			ui.ShowStats(_buildingController.statSprites, _buildingController.GetUniqueStats(), _buildingController.statsDescriptions);
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
            _buildingController.UpdateProductionQueue();
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
        if (_buildingController.startedProduction)
        {
            float percent = _buildingController.GetPercentageProduced();
            ui.productionProgressCanvas.UpdateProgress(percent);
        }

        else if (!_buildingController.HaveRequiredResourcesToProduce())
        {
            // Not enought resources == -1
			if (_buildingController.HaveEnoughtHousingAvailable ()) {
				ui.productionProgressCanvas.UpdateProgress (-1);
			} 

			else 
			{
				ui.productionProgressCanvas.UpdateProgress (-2);
			}
        }

        else
        {
            ui.productionProgressCanvas.UpdateProgress(0);
        }
    }

    public override void OnExit()
    {
        HideRallyPoint();

        ui.HideStats();
        ui.healthBar.HideHitpoints();
        ui.HideProductionButtons();

        ui.productionProgressCanvas.gameObject.SetActive(false);
        ui.productionQueueCanvas.gameObject.SetActive(false);
    }
}
