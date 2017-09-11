using UnityEngine;
using System.Collections;

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

    public int population = 0;
    public int housing = 0;
    public int newVillagers = 0;
    public int prosperity = 0;
    public int knowledge = 0;
    public int food;
    public int foodProduction = 0;
    public int timber = 0;
    public int stoneTools = 0;
    public int copper = 0;
    public int tin = 0;
    public int bronze = 0;

    [HideInInspector]
    public Color teamColor = Color.white;

    [HideInInspector]
    public FACTION faction = FACTION.SUMERIA;

    public int age = 0;

    public bool placedCamp = false;
}
