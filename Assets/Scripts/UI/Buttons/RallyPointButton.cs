using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyPointButton : UnitUIButton
{
    protected override void OnClick()
    {
        base.OnClick();

        PlayerManager.instance.SetRallyPointState();
    }
}
