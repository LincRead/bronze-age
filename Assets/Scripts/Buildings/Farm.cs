using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farm : Building
{
    public float harvestDifficulty = 1f;
    public bool hasFarmer = false;

    int soilQuality = 3;

    protected override void Start()
    {
        base.Start();
    }

    void CalculateSoilLevel()
    {
        List<Tile> tiles = Grid.instance.GetTilesOccupiedByController(this);

        if(tiles == null)
        {
            soilQuality = 0;
        }

        float soilQualityCalc = 0;

        for(int i = 0; i < tiles.Count; i++)
        {
            // In case we tried to get tiles outside of the map
            if (tiles[i] == null)
            {
                return;
            }

            switch (tiles[i].fertilityPoints)
            {
                case 1: soilQualityCalc += 0.5f; break;
                case 2: soilQualityCalc += 0.75f; break;
                case 3: soilQualityCalc += 1; break;
            }
        }

        soilQuality = Mathf.RoundToInt(soilQualityCalc);
        CalculateHarvestDifficulty();
    }

    void CalculateHarvestDifficulty()
    {
        harvestDifficulty = 4.25f - (soilQuality * 0.25f);
    }

    protected override void Place()
    {
        base.Place();

        ControllerUIManager.instance.cursorInformation.gameObject.SetActive(false);
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
        ControllerUIManager.instance.cursorInformation.UpdateValue(soilQuality);

        base.HandlePlacingBuilding();

    }

    protected override void SetConstructedSprite()
    {
        _spriteRenderer.sprite = constructionSprites[soilQuality];
    }

    public override int[] GetUniqueStats()
    {
        int[] stats = new int[1];
        stats[0] = soilQuality;
        return stats;
    }
}
