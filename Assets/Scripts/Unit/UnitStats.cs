using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Stats/Unit")]
public class UnitStats : DefaultStats
{
    [Header("Food consumption")]
    public int foodConsuming = 1;

    [Header("Health bar")]
    public GameObject healthBar;

    [Header("Unit stats")]
    public int maxHitpoints = 20;
    public int moveSpeed = 2;

    [Header("Attack values")]
    public bool canAttack = false;
    public int attackTriggerRadius = 6;

    [Header("Weapon type")]
    public WEAPON_TYPE weaponType;

    public enum WEAPON_TYPE
    {
        NONE,
        SPEAR,
        BOW,
        SLING,
        AXE,
        SWORD
    }

    [Header("Melee attack")]
    public bool canAttackMelee = false;
    public int damageMelee = 0;
    public float attackSpeedMelee = 1f;

    [Header("Ranged attack")]
    public bool canAttackRanged = false;
    public int damageRanged = 0;
    public float attackSpeedRanged = 1f;
    public int range = 0;
    public GameObject projectile;

    [Header("Siege attack")]
    public int damageSiege = 1;

    [Header("Defence")]
	public int rangedArmor = 0;
	public int meleeArmor = 0;
    public bool hasShield = false;
    public bool hasHeavyBodyArmor = false;

    [Header("Vilager")]
    public bool isVillager = false;

    [Header("Tribe")]
    public bool isTribe = false;
}
