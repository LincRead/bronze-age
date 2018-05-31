using UnityEngine;
using System.Collections;

public class BuildButton : UnitUIButton
{
    protected override void OnClick()
    {
        base.OnClick();

        Debug.Log("CLICKED BUID");

        if(PlayerManager.instance._controllerSelecting.selectedUnits.Count == 1)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDINGS, null);
        }

        else
        {
            EventManager.TriggerEvent("ActivateBuildingsView");
            EventManager.TriggerEvent("DisableUnitActionsView");
            EventManager.TriggerEvent("DisableVillagerView");
        }
    }
}
