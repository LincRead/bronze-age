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

        if(_controller._unitStats.damage == 0)
        {
            _controller.TransitionToState(_controller.idleState);
            return;
        }

        attackSpeed = _controller._unitStats.attackSpeed;
        _controller.FaceController(_controller.targetController);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        timeSinceAttack += Time.deltaTime;

        if(timeSinceAttack >= attackSpeed)
        {
            timeSinceAttack = 0.0f;

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
        // Destroyed, dead or moved
        return _controller.targetController != null
            && !_controller.targetController.dead
            && _controller.targetController.GetPrimaryNode() == _targetStandingOnNode;
    }

    protected virtual void StopAttack()
    {
        _controller.TransitionToState(_controller.idleState);
    }

    protected override void PlayAnimation()
    {
        _controller._animator.Play("attack", -1, 0.0f);
    }

    public override void CheckTransitions()
    {
        base.CheckTransitions();
    }
}