using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseTechTree : UnitUIButton {

	protected override void OnClick()
	{
		TechTreeManager.instance.Close();
	}
}
