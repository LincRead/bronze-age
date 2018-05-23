using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FACTION
{
    SUMERIA,
    ASSYRIAN,
    BABYLONIAN,
    EGYPTIAN,
    HITTIAN,
    MINOAN,
    MYCENEAEN
}

public class PlayerData {

	public float knowledgeGeneration = 0;
	public float knowledgeGenerated = 0;
    public int population = 0;
    public int housing = 0;
    public float foodInStock;
    public int foodIntake = 0;
    public int wealth = 0;
    public int timber = 0;
    public int metal = 0;

    // Food surplus level
    public int foodSurplusLevel = 0;

    // Need to always be paired number
    public int villagerCarryLimit = 10;

    [HideInInspector]
    public Color teamColor = Color.white;

    [HideInInspector]
    public FACTION faction = FACTION.SUMERIA;

    public int age = 0;

    public bool placedCamp = false;

    [HideInInspector]
    public List<Building> friendlyResourceDeliveryPoints = new List<Building>();

	public int numPriests = 0;

    // Technology stats
	public float knowledgeGenerationFactor = 1.0f;
    public int extraVillagerHP = 0;
    public float woodCuttingSpeed = 1f;
    public float gatherBerriesSpeed = 1f;
    public float gatherMeatSpeed = 1f;
    public float fishingSpeed = 1f;
    public float farmingSpeed = 1f;
    public float miningSpeed = 1f;
    public float militaryUnitTrainingSpeed = 1f;
}
