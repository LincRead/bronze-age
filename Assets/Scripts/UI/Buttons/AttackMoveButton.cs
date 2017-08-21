using UnityEngine;
using System.Collections;

public class AttackMoveButton : UnitUIButton
{
    protected override void OnClick()
    {
        PlayerManager.instance.SetAttackMoveState();
    }
}
