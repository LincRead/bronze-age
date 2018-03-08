using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Amount of resources player(s) start the game with.

[CreateAssetMenu(menuName = "Stats/Starting player resources")]
public class PlayerStartResources : ScriptableObject
{
    public int food = 0;
    public int timber = 0;
    public int wealth = 0;
    public int metal = 0;
	public int population = 5;
}