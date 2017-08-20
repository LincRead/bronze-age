using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Show info about selected building
[CreateAssetMenu(menuName = "UI/Controller views/building")]
public class BuildingView : ControllerUIView
{
    Building buildingsController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        Building buildingsController = controller.GetComponent<Building>();

        ui.ShowHitpoints(buildingsController.hitpoints, buildingsController.maxHitPoints);

        if (PlayerManager.myPlayerID == _controller.playerID)
        {
            ui.ShowStats(buildingsController.statSprites, buildingsController.GetUniqueStats());
        }
    }

    public override void Update()
    {

    }

    public override void OnExit()
    {
        ui.HideStats();
        ui.HideHitpoints();
    }
}
