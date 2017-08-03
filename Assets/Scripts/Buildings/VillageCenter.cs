using UnityEngine;
using System.Collections;

public class VillageCenter : Building
{
    [Header("Village Center stats")]
    public int housing = 5;

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
        int[] stats = new int[2];
        stats[0] = housing;
        stats[1] = 0; // Todo show available villagers resource?
        return stats;
    }
}
