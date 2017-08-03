using UnityEngine;
using System.Collections;

public class House : Building
{
    [Header("House values")]
    public int housing = 3;

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
