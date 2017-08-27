using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackUnitUIButton : UnitUIButton
{
    protected override void OnClick()
    {
        base.OnClick();

        if (PlayerManager.instance.currentUserState == PlayerManager.PLAYER_ACTION_STATE.PLACING_BUILDING)
            PlayerManager.instance.CancelPlaceBuildingState();

        if (PlayerManager.instance._controllerSelecting.selectedUnits.Count == 1)
        {
            ControllerUIManager.instance.GoBackToLastView();
        }

        else
        {
            EventManager.TriggerEvent("ActivateUnitActionsView");
            EventManager.TriggerEvent("ActivateVillagerView");

            EventManager.TriggerEvent("DisableBuildingsView");
        }
    }
}