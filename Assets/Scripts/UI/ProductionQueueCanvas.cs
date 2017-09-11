using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionQueueCanvas : MonoBehaviour
{
    public GameObject[] productionQueueIcons;

    [HideInInspector]
    public static int max = 5;

    string text;

    public void ActivateIcon(Sprite newIconSprite, int index)
    {
        productionQueueIcons[index].GetComponent<Icon>().icon.enabled = true;
        productionQueueIcons[index].GetComponent<Icon>().icon.sprite = newIconSprite;

        productionQueueIcons[index].GetComponentInChildren<Text>().enabled = false;
    }

    public void DeactivateIcon(int index)
    {
        productionQueueIcons[index].GetComponentInChildren<Text>().enabled = true;

        productionQueueIcons[index].GetComponent<Icon>().icon.enabled = false;
    }
}
