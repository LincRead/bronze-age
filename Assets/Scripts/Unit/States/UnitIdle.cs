using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/idle")]
public class UnitIdle : UnitState
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        controller.targetController = null;
        _controller.StartCoroutine("DetectNearbyEnemies");
    }

    protected override void PlayAnimation()
    {
        if (!_controller._animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            _controller._animator.Play("idle", -1, 0.0f);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _controller.StopAllCoroutines();
    }
}
