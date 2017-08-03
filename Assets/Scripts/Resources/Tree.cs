using UnityEngine;
using System.Collections;
using System.Text;

public class Tree : Resource {

    protected override void UpdateResourceAmountForPlayer(int playerID)
    {
        PlayerDataManager.instance.AddTimberForPlayer(1, playerID);
    }
}
