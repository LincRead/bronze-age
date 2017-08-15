using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/attack")]
public class UnitAttack : UnitState
{
    float attackSpeed;
    float timeSinceAttack = 0.0f;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        attackSpeed = _controller._unitStats.attackSpeed;

        PlayAttackAnimation();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        timeSinceAttack += Time.deltaTime;
        if(timeSinceAttack >= attackSpeed)
        {
            PlayAttackAnimation();
            timeSinceAttack = 0.0f;
        }
    }

    void PlayAttackAnimation()
    {
        if(_controller.targetController == null || _controller.targetController.dead)
            _controller.TransitionToState(_controller.idleState);
        else
            _controller._animator.Play("attack", -1, 0.0f);
    }
}