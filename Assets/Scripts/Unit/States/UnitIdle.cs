using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "States/Unit states/idle")]
public class UnitIdle : UnitState
{
    bool setToIdle = false;
    float timeUntilSetIdle = 1f;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        controller.targetController = null;
        _controller.StartCoroutine("DetectNearbyEnemies");

        // Reset
        setToIdle = false;
    }

    protected override void PlayAnimation()
    {
        _controller.PlayIdleAnimation();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (_controller.playerID == PlayerManager.myPlayerID && !setToIdle)
        {
            if (_controller._unitStats.isVillager && timeSinceStateChange >= timeUntilSetIdle)
            {
                PlayerManager.instance.idleVillagers.Add(_controller);
                setToIdle = true;
            }
        }
    }

    public override void OnExit()
    {
        if (_controller.playerID == PlayerManager.myPlayerID)
        {
            if (setToIdle)
            {
                PlayerManager.instance.idleVillagers.Remove(_controller);
            }
        }

        base.OnExit();

        _controller.StopAllCoroutines();
    }
}
