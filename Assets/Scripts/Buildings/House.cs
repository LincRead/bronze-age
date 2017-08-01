using UnityEngine;
using System.Collections;

public class House : Building
{
    [Header("House stats")]
    public int housing = 3;

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
        int[] stats = new int[1];
        stats[0] = housing;
        return stats;
    }
}
