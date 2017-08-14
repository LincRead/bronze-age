using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/idle")]
public class UnitIdle : UnitState
{
    public override void OnEnter(UnitStateController controller)
    {
        controller._animator.Play("idle");
        controller.targetController = null;
    }
}
