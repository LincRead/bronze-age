using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Building")]
public class BuildingStats : DefaultStats
{
    [Header("Health bar")]
    public GameObject healthBar;

    [Header("Building stats")]
    public int maxHitpoints = 100;
    public int visionRange = 10;

    [Header("Construction")]
    [SerializeField]
    public float stepsToConstruct = 3f;
    public Sprite[] constructionSprites = new Sprite[3];

    [Header("Damaged visuals")]
    public Sprite[] damagedSprites = new Sprite[2];

    [Header("Production")]
    public ProductionButtonData[] productionButtons;

    [Header("Main building")]
    public bool isCivilizationCenter = false;
}
