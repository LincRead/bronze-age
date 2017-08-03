using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Amount of resources player(s) start the game with.

[CreateAssetMenu(menuName = "Stats/Starting player resources")]
public class PlayerStartResources : ScriptableObject
{
    public int food = 0;
    public int timber = 0;
    public int stone = 0;
    public int copper = 0;
    public int tin = 0;
    public int bronze = 0;
}
