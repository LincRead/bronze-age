using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Weaving")]
public class Weaving : FinishedResearchAction
{
    public int extraVillagerHP = 5;

	public override void ActivateTechnology(int playerID)
    {
		PlayerDataManager.instance.GetPlayerData(playerID).extraVillagerHP = this.extraVillagerHP;

        // Update all Villager hitpoints
        List<UnitStateController> friendlyUnits = PlayerManager.instance.friendlyUnits;
		int addHitpoints = PlayerDataManager.instance.GetPlayerData(playerID).extraVillagerHP;

        for (int i = 0; i < friendlyUnits.Count; i++)
        {
            if(friendlyUnits[i]._unitStats.isVillager)
            {
                friendlyUnits[i].maxHitpoints += addHitpoints;
                friendlyUnits[i].hitpointsLeft += addHitpoints;
                friendlyUnits[i].UpdateHealthBar();
            }
        }
    }
}
