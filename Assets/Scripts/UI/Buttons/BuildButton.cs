using UnityEngine;
using System.Collections;

public class BuildButton : UnitUIButton
{
    protected override void OnClick()
    {
        ControllerUIManager.instance.ChangeUI(ControllerUIManager.CONTROLLER_UI_TYPE.BUILDINGS);
    }
}
