using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Finished Production Actions/Advance Age")]
public class AdvanceAgeAction : FinishedProductionAction
{
    public override void Action(Building building)
    {
        PlayerManager.instance.currentAge++;
        EventManager.TriggerEvent("AdvancedCivilizationAge");
    }
}
