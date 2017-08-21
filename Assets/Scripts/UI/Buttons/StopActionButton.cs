using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopActionButton : UnitUIButton
{
    protected override void OnClick()
    {
        PlayerManager.instance.StopAction();
    }
}
