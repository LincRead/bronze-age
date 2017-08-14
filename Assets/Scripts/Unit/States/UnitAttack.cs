using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/attack")]
public class UnitAttack : UnitState
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        controller._animator.Play("idle");
    }
}