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

    public int newCitizens = 0;
    public int population = 0;
    public int housing = 0;
    public int staticProsperity = 1;
    public float foodStock;
    public int foodIntake = 0;
    public int timber = 0;
    public int metal = 0;

    [HideInInspector]
    public Color teamColor = Color.white;

    [HideInInspector]
    public FACTION faction = FACTION.SUMERIA;

    public int age = 0;

    public bool placedCamp = false;

    [HideInInspector]
    public float progressTowardsNewCitizen = 0.0f;

    [HideInInspector]
    public int realProsperity = 0;
}
