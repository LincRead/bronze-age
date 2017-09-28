using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionQueueCanvas : MonoBehaviour
{
    public GameObject[] productionQueueButtons;

    Building currentProductionController;

    public static int max = 5;

    private void Start()
    {
        for(int i = 0; i < productionQueueButtons.Length; i++)
        {
            productionQueueButtons[i].GetComponent<ProductionQueueButton>().SetReferenceToParent(this);
        }
    }

    public void UpdateData(Building building)
    {
        currentProductionController = building;

        for (int i = 0; i < max; i++)
        {
            if (i < currentProductionController.productionList.Count)
            {
                productionQueueButtons[i].GetComponent<ProductionQueueButton>().ActivateIcon(currentProductionController.productionButtonsData[currentProductionController.productionList[i]].icon);
            }

            else
            {
                productionQueueButtons[i].GetComponent<ProductionQueueButton>().DeactivateIcon();
            }
        }
    }

    public void CancelProduction(int index)
    {
        currentProductionController.RemoveProductionAtQueue(index);
    }
}
