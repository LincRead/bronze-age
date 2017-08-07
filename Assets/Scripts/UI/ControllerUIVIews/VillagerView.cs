using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller views/villager")]
public class VillagerView : UnitView
{
    UnitStateController unitController;

    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        EventManager.TriggerEvent("ActivateVillagerView");
    }

    public override void OnExit()
    {
        EventManager.TriggerEvent("DisableVillagerView");
    }
}
