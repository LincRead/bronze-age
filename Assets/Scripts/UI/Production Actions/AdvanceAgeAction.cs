using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Finished Production Actions/Advance Age")]
public class AdvanceAgeAction : FinishedProductionAction
{
    public override void Action(Building building)
    {
        // Update age for my player
        PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).age++;

        // Keep in synch
        PlayerManager.instance.currentAge++;

        PlayerManager.instance.civilizationCenter.Upgrade();
    }
}
