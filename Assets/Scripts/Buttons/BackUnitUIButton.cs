using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackUnitUIButton : UnitUIButton
{
    // Update is called once per frame
    protected override void OnClick()
    {
        if(WorldManager.instance.currentUserState == WorldManager.USER_STATE.PLACING_BUILDING)
            WorldManager.instance.CancelBuildPlacebuild();

        UnitUIManager.instance.GoBack();
    }
}