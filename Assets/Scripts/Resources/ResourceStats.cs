using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Resource")]
public class ResourceStats : DefaultStats {

    [Header("Resource stats")]
    public HARVEST_TYPE harvestType;
    public RESOURCE_TYPE resourceType;
    public int amount = 10;
    public float harvestDifficulty = 1;

    [Header("Harvest visuals")]
    public Sprite[] harvestStagesSprites = new Sprite[0];
}
