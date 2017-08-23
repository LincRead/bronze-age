using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/idle")]
public class UnitIdle : UnitState
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);
        controller._animator.Play("idle", -1, 0.0f);
        controller.targetController = null;
        _controller.StartCoroutine("DetectNearbyEnemies");
    }

    public override void OnExit()
    {
        base.OnExit();

        _controller.StopAllCoroutines();
    }
}
