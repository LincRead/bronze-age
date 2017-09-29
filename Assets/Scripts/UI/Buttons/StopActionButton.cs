using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StopActionButton : UnitUIButton
{
    protected override void OnClick()
    {
        PlayerManager.instance.StopAction();

        if(isActiveAndEnabled)
        {
            ShowTooltip();
        }
    }
}
