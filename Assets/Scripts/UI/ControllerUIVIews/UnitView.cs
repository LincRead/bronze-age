using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : ControllerUIView {

    UnitStateController unitController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        if(controller != null)
        {
            unitController = controller.GetComponent<UnitStateController>();
            ui.ShowHitpoints(unitController.hitpointsLeft, unitController._unitStats.maxHitpoints);
            ui.ShowStats(_controller.statSprites, _controller.GetUniqueStats());

            if (_controller.playerID == PlayerManager.myPlayerID)
            {
                EventManager.TriggerEvent("ActivateUnitActionsView");
            }
        }
    }

    public override void Update()
    {
        ui.UpdateHitpoints(unitController.hitpointsLeft, unitController._unitStats.maxHitpoints);
    }

    public override void OnExit()
    {
        ui.HideHitpoints();
        ui.HideStats();
        ui.HideStats();

        if (_controller.playerID == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("DisableUnitActionsView");
        }
    }
}
