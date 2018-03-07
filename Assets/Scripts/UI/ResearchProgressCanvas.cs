using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ResearchProgressCanvas : MonoBehaviour {

	[Header("Production progress")]
	public Image productionProgressBarImage;
	public Image icon;

	public void UpdateProgress(float percent)
	{
		if(percent != -1)
		{
			productionProgressBarImage.fillAmount = percent;
		}

		else
		{
			productionProgressBarImage.fillAmount = 0;
		}
	}
}
