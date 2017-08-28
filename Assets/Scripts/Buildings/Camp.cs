using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp : CivilizationCenter
{
    protected override void Place()
    {
        base.Place();

        constructed = true;
        PlayerDataManager.instance.GetPlayerData(playerID).placedCamp = true;
    }
}
