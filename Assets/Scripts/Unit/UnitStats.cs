using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Stats/Unit stats")]
public class UnitStats : ScriptableObject {

    public string title = "Unit";
    public int maxHitpoints = 3;
    public int damage = 1;
    public float attackSpeed = 1f;
    public float moveSpeed = 20f;
    public int visionRange = 8;
    public int attackRange = 5;

    public bool builder = false;
    public bool gatherer = false;
}
