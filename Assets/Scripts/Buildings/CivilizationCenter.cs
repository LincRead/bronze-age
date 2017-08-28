using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationCenter : Building
{
    [Header("Unique Stats")]
    public int housing = 10;

    protected override void Start()
    {
        base.Start();

        PlayerManager.instance.civilizationCenter = this;
    }

    protected override void AddPlayerStats()
    {
        PlayerDataManager.instance.AddHousingForPlayer(housing, playerID);
    }

    protected override void RemovePlayerStats()
    {
        PlayerDataManager.instance.AddHousingForPlayer(-housing, playerID);
    }

    public override int[] GetUniqueStats()
    {
        int[] stats = new int[1];
        stats[0] = housing;
        return stats;
    }
}
