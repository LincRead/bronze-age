using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Stats/Unit")]
public class UnitStats : DefaultStats
{
    [Header("Unit stats")]
    public int maxHitpoints = 3;
    public int damage = 1;
    public float attackSpeed = 1f;
    public float moveSpeed = 20f;
    public int visionRange = 8;
    public int attackTriggerRadius = 6;

    [Header("Ranged")]
    public bool isRanged = false;
    public int range = 0;
    public GameObject projectile;

    [Header("Vilager")]
    public bool isVillager = false;
}
