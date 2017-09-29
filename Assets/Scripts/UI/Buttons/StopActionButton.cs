using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StopActionButton : UnitUIButton
{
    protected override void OnClick()
    {
        PlayerManager.instance.StopAction();

        // If button is still here after cancelling production,
        // make sure we continue to show Tooltip
        if(isActiveAndEnabled)
        {
            ShowTooltip();
        }
    }
}
