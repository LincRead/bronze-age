using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TribeView : UnitView
{
    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        if (_controller.playerID == PlayerManager.myPlayerID
            && !_controller.GetComponent<TribeController>().movingTowarsCamp)
        {
            EventManager.TriggerEvent("ActivateTribeView");
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_controller.playerID == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("DisableTribeView");
        }
    }
}
