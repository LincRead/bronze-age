using UnityEngine;
using System.Collections;

public class BuildButton : UnitUIButton
{
    // Update is called once per frame
    protected override void OnClick()
    {
        UnitUIManager.instance.ChangeUI(UnitUIManager.UNIT_UI_TYPE.BUILDINGS);
    }

    // Hotkey for B
}
