using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller views/production")]
public class ProductionView : ControllerUIView
{
    Building _buildingController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        _buildingController = controller.GetComponent<Building>();

        EventManager.TriggerEvent("ActivateProgressView");
        EventManager.TriggerEvent("ActivateQueueSlotsView");
        UpdatePercentProductionVisuals();
    }

    public override void Update()
    {
        UpdatePercentProductionVisuals();
    }

    void UpdatePercentProductionVisuals()
    {
        float percent = _buildingController.GetPercentageProduced();
        ui.productionProgressText.text = new StringBuilder((int)(percent * 100) + "%").ToString();
        ui.productionProgressBarImage.fillAmount = percent;
    }

    public override void OnExit()
    {
        EventManager.TriggerEvent("DisableProgressView");
        EventManager.TriggerEvent("DisableQueueSlotsView");
    }
}