using UnityEngine;
using System.Collections;

public class VillageCenter : Building
{
    [Header("Village Center stats")]
    public int housing = 5;

    protected override void FinishConstruction()
    {
        base.FinishConstruction();
        WorldManager.Manager.GetPlayerDataForPlayer(playerID).housing += housing;
        WorldManager.Manager.UpdateHousingText();
    }

    public override void Destroy()
    {
        WorldManager.Manager.GetPlayerDataForPlayer(playerID).housing -= housing;
        WorldManager.Manager.UpdateHousingText();
        base.Destroy();
    }

    public override int[] GetUniqueStats()
    {
        int[] stats = new int[2];
        stats[0] = housing;
        stats[1] = 0;
        return stats;
    }
}
