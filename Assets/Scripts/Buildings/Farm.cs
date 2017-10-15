﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farm : Building
{
    public int harvestDifficulty = 1;
    public bool hasFarmer = false;

    int randomSpriteIndex = 1;

    int soilQuality = 3;

    protected override void Start()
    {
        float ran = Random.value;
        randomSpriteIndex = (1 + Mathf.RoundToInt(ran));

        base.Start();
    }

    void CalculateSoilLevel()
    {
        List<Tile> tiles = Grid.instance.GetTilesOccupiedByController(this);

        float soilQualityCalc = 0;

        for(int i = 0; i < tiles.Count; i++)
        {
            switch(tiles[i].fertilityPoints)
            {
                case 1: soilQualityCalc += 0.5f; break;
                case 2: soilQualityCalc += 0.75f; break;
                case 3: soilQualityCalc += 1; break;
            }
        }

        soilQuality = Mathf.RoundToInt(soilQualityCalc);
    }

    protected override void AdditionalCanPlaceChecks()
    {
        if(soilQuality <= 0)
        {
            canPlace = false;
        }
    }

    protected override void HandlePlacingBuilding()
    {

        CalculateSoilLevel();

        base.HandlePlacingBuilding();

    }

    protected override void SetConstructedSprite()
    {
        _spriteRenderer.sprite = constructionSprites[randomSpriteIndex];
    }

    public override int[] GetUniqueStats()
    {
        int[] stats = new int[1];
        stats[0] = soilQuality;
        return stats;
    }
}