using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller views/construction")]
public class ConstructionView : ControllerUIView
{
    Building _buildingController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        _buildingController = controller.GetComponent<Building>();

        // Since Camp is instantly constructed when Tribe reached building,
        // don't show construction progress for Camp
        // Show for all other buildings being constructed
        if (!_buildingController._buildingStats.isCivilizationCenter)
        {
            ui.productionProgressCanvas.gameObject.SetActive(true);
            UpdatePercentConstructedVisuals();
        }

        ui.healthBar.ShowHitpoints(_buildingController.hitpointsLeft, _buildingController.maxHitPoints);

        // Show Rally Point button?
        SetRallyPointButtonActive();
    }

    void SetRallyPointButtonActive()
    {
        if (_buildingController.canSetRallyPoint)
        {
            ControllerUIManager.instance.rallyPointButton.gameObject.SetActive(true);
        }

        else
        {
            ControllerUIManager.instance.rallyPointButton.gameObject.SetActive(false);
        }
    }

    public override void Update()
    {
        if (!_buildingController._buildingStats.isCivilizationCenter)
        {
            UpdatePercentConstructedVisuals();
        }

        ui.healthBar.UpdateHitpoints(_buildingController.hitpointsLeft, _buildingController.maxHitPoints);
    }

    void UpdatePercentConstructedVisuals()
    {
        float percent = _buildingController.GetPercentageProduced();
        ui.productionProgressCanvas.UpdateProgress(percent);
    }

    public override void OnExit()
    {
        ui.productionProgressCanvas.gameObject.SetActive(false);
        ui.healthBar.HideHitpoints();
    }
}
