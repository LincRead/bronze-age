using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Stats/Unit")]
public class UnitStats : DefaultStats
{
    [Header("Health bar")]
    public GameObject healthBar;

    [Header("Unit stats")]
    public int maxHitpoints = 20;
    public float moveSpeed = 0.5f;

    [Header("Attack stats")]
    public bool canAttack = true;
    public int damage = 1;
    public float attackSpeed = 1f;
    public int attackTriggerRadius = 6;

    [Header("Ranged attack")]
    public bool isRanged = false;
    public int range = 0;
    public GameObject projectile;

    [Header("Vilager")]
    public bool isVillager = false;

    [Header("Tribe")]
    public bool isTribe = false;
}
