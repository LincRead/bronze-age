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
        if(!_buildingController._buildingStats.isCivilizationCenter)
        {
            EventManager.TriggerEvent("ActivateConstructionView");
            UpdatePercentConstructedVisuals();
        }
    }

    public override void Update()
    {
        if (!_buildingController._buildingStats.isCivilizationCenter)
        {
            UpdatePercentConstructedVisuals();
        }
    }

    void UpdatePercentConstructedVisuals()
    {
        float percent = _buildingController.GetPercentageConstructed();
        ui.constructionProgressText.text = new StringBuilder((int)(percent * 100) + "%").ToString();
        ui.constructionProgressBarImage.fillAmount = percent;
    }

    public override void OnExit()
    {
        EventManager.TriggerEvent("DisableConstructionView");
    }
}
