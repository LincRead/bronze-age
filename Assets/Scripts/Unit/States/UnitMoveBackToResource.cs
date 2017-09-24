using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveBackToResource : UnitMoveToController
{
    // Make sure we move to resource pos if target Resource gets destroyed 
    // before we reach it.
    protected override void ReachedNextTargetNode()
    {
        if (_controller.targetController == null)
        {
            _controller.MoveToResourcePos(_targetControllerPosition);
            return;
        }

        base.ReachedNextTargetNode();
    }
}
