using UnityEngine;
using System.Collections;

public class BuildButton : UnitUIButton
{
    protected override void OnClick()
    {
        ControllerUIManager.instance.ChangeUI(ControllerUIManager.UNIT_UI_TYPE.BUILDINGS);
    }
}
