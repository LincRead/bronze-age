using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ProductionProgressCanvas : MonoBehaviour {

    [Header("Production progress")]
    public Image productionProgressBarImage;
    public Text productionProgressText;
    public Image icon;

    // Use this for initialization
    public void UpdateProgress(float percent)
    {
        productionProgressText.text = new StringBuilder((int)(percent * 100) + "%").ToString();
        productionProgressBarImage.fillAmount = percent;
    }
}
