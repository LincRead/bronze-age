using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologyButtonsCanvas : MonoBehaviour {

	public GameObject technologyButtonPrefab;
	TechnologyButton[] technologyButtonsList = new TechnologyButton[18];

	void Start ()
	{
		int index = 0;
		for(int i = 0; i < 3; i++)
		{
			for(int j = 0; j < 6; j++)
			{
				GameObject newButton = GameObject.Instantiate(technologyButtonPrefab, gameObject.transform);
				newButton.GetComponent<RectTransform>().localPosition = new Vector3(34 * j, - 34 * i);
				technologyButtonsList[index] = newButton.GetComponent<TechnologyButton>();
				index++;
			}
		}

		TechTreeManager.instance.SetupTechnologyButtonsView(technologyButtonsList);
	}
}
