using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/die")]
public class UnitDie : UnitState
{
    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _controller._animator.Play("die");
        _controller.dead = true;
    }

    public override void CheckTransitions()
    {
        // Let die animation finish
        if (timeSinceStateChange > _controller._animator.GetCurrentAnimatorStateInfo(0).length)
        {
            Destroy(_controller.gameObject);
        }
    }
}
