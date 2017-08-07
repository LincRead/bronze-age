using UnityEngine;
using System.Collections;

public class BuildButton : UnitUIButton
{
    protected override void OnClick()
    {
        ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDINGS, null);
    }
}
