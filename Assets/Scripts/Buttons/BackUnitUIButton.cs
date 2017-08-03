using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackUnitUIButton : UnitUIButton
{
    // Update is called once per frame
    protected override void OnClick()
    {
        if(PlayerManager.instance.currentUserState == PlayerManager.PLAYER_ACTION_STATE.PLACING_BUILDING)
            PlayerManager.instance.CancelBuildPlacebuild();

        UnitUIManager.instance.GoBack();
    }
}