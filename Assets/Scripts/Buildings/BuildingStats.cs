using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Building")]
public class BuildingStats : DefaultStats
{
    [Header("Health bar")]
    public GameObject healthBar;

    [Header("Building stats")]
    public int maxHitpoints = 100;

    [Header("Construction")]
    [SerializeField]
    public float stepsToConstruct = 3f;
    public Sprite[] constructionSprites = new Sprite[2];

    [Header("Damaged visuals")]
    public Sprite damagedSprite;

    [Header("Production")]
    public ProductionButtonData[] productionButtons;

    [Header("Main building")]
    public bool isCivilizationCenter = false;

    [Header("Can deliver materials to this building")]
    public bool deliveryPointMaterials = false;

    [Header("Can deliver food to this building")]
    public bool deliveryPointFood = false;

    [HideInInspector]
    public int food = 0;

    [HideInInspector]
    public int wealth = 0;

    [HideInInspector]
    public int timber = 0;

    [HideInInspector]
    public int metal = 0;

    [HideInInspector]
    public int newCitizens = 0;
}
