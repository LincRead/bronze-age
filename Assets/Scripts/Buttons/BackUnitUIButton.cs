using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackUnitUIButton : UnitUIButton
{
    // Update is called once per frame
    protected override void OnClick()
    {
        if(WorldManager.Manager.currentUserState == WorldManager.USER_STATE.PLACING_BUILDING)
            WorldManager.Manager.CancelBuildPlacebuild();

        UnitUIManager.Manager.GoBack();
    }
}