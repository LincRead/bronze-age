using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller views/villager")]
public class VillagerView : UnitView
{
    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        if(_controller.playerID == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("ActivateVillagerView");
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_controller.playerID == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("DisableVillagerView");
        }
    }
}
