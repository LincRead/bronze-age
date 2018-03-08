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

    public void UpdateProgress(float percent)
    {
        if(percent > -1)
        {
            productionProgressText.text = new StringBuilder((int)(percent * 100) + "%").ToString();
            productionProgressText.color = new Color32(0x8F, 0x7E, 0x60, 0xFF);
            productionProgressText.fontSize = 14;

            productionProgressBarImage.fillAmount = percent;
        }

		else 
        {
			if (percent == -2) 
			{
				productionProgressText.text = new StringBuilder ("Lack of housing").ToString ();

			} 

			else 
			{
				productionProgressText.text = new StringBuilder ("Not enought resources").ToString ();
			}
            
            productionProgressText.color = new Color32(0xE5, 0x48, 0x48, 0xFF);
            productionProgressText.fontSize = 10;

            productionProgressBarImage.fillAmount = 0;
        }
    }
}
