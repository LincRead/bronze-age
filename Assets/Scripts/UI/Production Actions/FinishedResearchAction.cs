using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedResearchAction : ScriptableObject {

	public virtual void ActivateTechnology(int playerID)
	{
		Debug.Log("Finished production - Execute Action!");
	}
}
