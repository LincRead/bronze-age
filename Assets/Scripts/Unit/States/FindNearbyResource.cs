using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Unit states/find nearby resource")]
public class FindNearbyResource : UnitState
{
    Resource _resource;

    public override void OnEnter(UnitStateController controller)
    {
        base.OnEnter(controller);

        _resource = _controller.targetController.GetComponent<Resource>();

        Debug.Log("LOOK FOR RESOURCE");
    }
}
