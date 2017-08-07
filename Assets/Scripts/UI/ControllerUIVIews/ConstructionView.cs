using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller views/construction")]
public class ConstructionView : ControllerUIView
{
    Building buildingsController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        EventManager.TriggerEvent("ActivateConstructionView");

        buildingsController = controller.GetComponent<Building>();
    }

    public override void Update()
    {
        float percent = buildingsController.GetPercentageConstructed();
        ui.constructionProgressText.text = ((int)(percent * 100)).ToString() + "%";
        ui.constructionProgressBarImage.fillAmount = percent;
    }

    public override void OnExit()
    {
        EventManager.TriggerEvent("DisableConstructionView");
    }
}
