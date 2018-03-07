using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionButtonsCanvas : MonoBehaviour {

    public GameObject productionButtonPrefab;
    ProductionButton[] productionButtonsList = new ProductionButton[15];

	void Start ()
    {
        int index = 0;
		for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                GameObject newButton = GameObject.Instantiate(productionButtonPrefab, gameObject.transform);
                newButton.GetComponent<RectTransform>().localPosition = new Vector3(34 * j, - 34 * i);
                productionButtonsList[index] = newButton.GetComponent<ProductionButton>();
                index++;
            }
        }

        ControllerUIManager.instance.SetupProductionButtonsView(productionButtonsList);
	}
}
