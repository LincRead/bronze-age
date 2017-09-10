using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionTooltip : MonoBehaviour {

    public Text title;
    public Text description;

    public Image[] resourceIcons;
    public Text[] resourceAmounts;

    public void UpdateData(ProductionButtonData data)
    {
        title.text = data.title;
        description.text = data.description;
    }
}
