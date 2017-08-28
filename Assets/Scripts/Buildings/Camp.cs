using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp : CivilizationCenter
{
    protected override void Place()
    {
        base.Place();

        PlayerDataManager.instance.GetPlayerData(playerID).placedCamp = true;
    }

    public override void FinishConstruction()
    {
        // Instantly constructed, so update hitpoints instantly too
        hitpointsLeft = maxHitPoints;

        title = "Camp";

        if(selected)
        {
            ControllerUIManager.instance.UpdateTitle(title);
        }

        base.FinishConstruction();
    }
}
