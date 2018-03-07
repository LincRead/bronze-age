using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenTechTree : UnitUIButton
{
	protected override void OnClick()
	{
		TechTreeManager.instance.Open();
	}
}