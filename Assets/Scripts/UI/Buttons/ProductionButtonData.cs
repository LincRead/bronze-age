using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PRODUCTION_TYPE
{
    DEFAULT,
    BUILDING,
    TECHNOLOGY,
    UNIT
}

[CreateAssetMenu(menuName = "Stats/Production Data")]
public class ProductionButtonData : ScriptableObject {

    public PRODUCTION_TYPE type = PRODUCTION_TYPE.DEFAULT;

    [Header("Execute code on action")]
    public FinishedProductionAction executeScript;

    [Header("Building")]
    public GameObject productionPrefab;

    [Header("Icon")]
    public Sprite icon;

    [Header("Civilization Age")]
    public int age;

    [Header("Required resources")]
    public int timber = 0;
    public int stone = 0;
    public int food = 0;
    public int copper = 0;
    public int bronze = 0;
}
