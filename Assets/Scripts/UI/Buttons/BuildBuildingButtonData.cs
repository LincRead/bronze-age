using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Build Building Button")]
public class BuildBuildingButtonData : ScriptableObject {

    [Header("Building")]
    public GameObject buildingPrefab;

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
