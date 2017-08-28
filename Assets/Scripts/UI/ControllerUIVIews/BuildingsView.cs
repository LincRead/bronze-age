using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Show button for buildings selected villager can construct
[CreateAssetMenu(menuName = "UI/Controller views/buildings")]
public class BuildingsView : UnitView
{
    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        EventManager.TriggerEvent("ActivateBuildingsView");
        EventManager.TriggerEvent("DisableUnitActionsView");
    }

    public override void OnExit()
    {
        base.OnExit();

        EventManager.TriggerEvent("DisableBuildingsView");
    }
}
