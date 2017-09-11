﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PRODUCTION_TYPE
{
    DEFAULT,
    BUILDING,
    TECHNOLOGY,
    UNIT
}

[CreateAssetMenu(menuName = "UI/Production Button Data")]
public class ProductionButtonData : ScriptableObject {

    public PRODUCTION_TYPE type = PRODUCTION_TYPE.DEFAULT;

    [Header("Title")]
    public string title;

    [Header("Description")]
    public string description;

    [Header("Execute when production finishes")]
    public FinishedProductionAction executeScript;

    [Header("Produce")]
    public GameObject productionPrefab;

    [Header("Icon")]
    public Sprite icon;

    [Header("View position")]
    public int index = 0;

    [Header("Production time")]
    public int stepsRequired = 3;

    [Header("Required Civilization Age")]
    public int age = 0;

    [Header("Required technology")]
    public string requiredTechnology;

    [Header("Required resources")]
    public int timber = 0;
    public int stoneTools = 0;
    public int food = 0;
    public int copper = 0;
    public int bronze = 0;
    public int population = 0;
}
