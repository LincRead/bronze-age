using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/attack")]
public class UnitAttack : UnitState
{
    protected float attackSpeed;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        SetAttackSpeed();

        // Make sure it's not too close to last attack.
        if (_controller.timeSinceLastAttack < attackSpeed)
        {
            playAnimationAtStart = false;
            _controller._animator.Play("idle", -1, 0.0f);
        }
        
        _targetStandingOnNode = controller.targetController.GetPrimaryNode();
        _controller.FaceController(_controller.targetController);
    }

    protected virtual void SetAttackSpeed()
    {
        attackSpeed = _controller._unitStats.attackSpeedMelee;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if(_controller.timeSinceLastAttack >= attackSpeed)
        {
            if (ContinueToAttack())
            {
                PlayAnimation();
            }
               
            else
            {
                StopAttack();
            }
        }
    }

    protected virtual bool ContinueToAttack()
    {
        // Destroyed, dead or moved?
        return _controller.targetController != null && !_controller.targetController.dead;
    }

    protected virtual void StopAttack()
    {
        _controller.TransitionToState(_controller.idleState);
    }

    protected override void PlayAnimation()
    {
        _controller._animator.speed = 1.0f;
        _controller.timeSinceLastAttack = 0.0f;
        _controller._animator.Play("strike", -1, 0.0f);
    }

    public override void OnExit()
    {
        base.OnExit();

        // Reset
        _controller.distanceToTarget = 1000;

        // Make sure we don't stay frozen
        _controller._animator.speed = 1.0f;
    }
}