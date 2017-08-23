﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnitMoveToController : UnitMoveToController
{
    protected override void IntersectingTarget()
    {
        if (_controller.distanceToTarget <= _controller._unitStats.range * 10)
        {
            _controller.TransitionToState(_controller.rangedAttackState);
        }
    }
}
