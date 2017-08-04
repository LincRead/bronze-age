using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackUnitUIButton : UnitUIButton
{
    protected override void OnClick()
    {
        if(PlayerManager.instance.currentUserState == PlayerManager.PLAYER_ACTION_STATE.PLACING_BUILDING)
            PlayerManager.instance.CancelPlaceBuildingState();

        ControllerUIManager.instance.GoBack();
    }
}