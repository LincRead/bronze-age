using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Technologies/Weaving")]
public class Weaving : FinishedProductionAction
{
    public int extraVillagerHP = 5;

    public override void Action(Building building)
    {
        int id = building.playerID;

        PlayerDataManager.instance.GetPlayerData(id).extraVillagerHP = this.extraVillagerHP;

        // Update all Villager hitpoints
        List<UnitStateController> friendlyUnits = PlayerManager.instance.friendlyUnits;
        for(int i = 0; i < friendlyUnits.Count; i++)
        {
            if(friendlyUnits[i]._unitStats.isVillager)
            {
                friendlyUnits[i].maxHitpoints += PlayerDataManager.instance.GetPlayerData(id).extraVillagerHP;
                friendlyUnits[i].UpdateHealthBar();
            }
        }
    }
}
