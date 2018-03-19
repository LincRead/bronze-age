using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller views/resource")]
public class ResourceView : ControllerUIView
{
    Resource resourceController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        resourceController = controller.GetComponent<Resource>();

		ui.ShowStats(resourceController.statSprites, resourceController.GetUniqueStats(), resourceController.statsDescriptions);
    }

    public override void Update()
    {

    }

    public override void OnExit()
    {
        ui.HideStats();
    }
}
