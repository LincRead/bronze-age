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

        _controller.FaceController(_controller.targetController);

        PlayAttackAnimation();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        timeSinceAttack += Time.deltaTime;
        if(timeSinceAttack >= attackSpeed)
        {
            timeSinceAttack = 0.0f;

            if (_controller.targetController == null
                || _controller.targetController.dead
                // Has moved
                || _controller.targetController.GetPrimaryNode() != _targetStandingOnNode)
                _controller.TransitionToState(_controller.idleState);
            else
                PlayAttackAnimation();
        }
    }

    void PlayAttackAnimation()
    {
        if (!_controller._animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
            _controller._animator.Play("attack");
        else
            _controller._animator.Play("attack", -1, 0.0f);
    }

    public override void CheckTransitions()
    {
        base.CheckTransitions();
    }
}