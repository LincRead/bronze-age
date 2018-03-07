using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class TechnologyProgressCanvas : MonoBehaviour {

	[Header("Technology progress")]
	public Image technologyProgressBarImage;
	public Text technologyProgressText;
	public Image icon;

	public void UpdateProgress(float percent)
	{
		if(percent != -1)
		{
			technologyProgressText.text = new StringBuilder((int)(percent * 100) + "%").ToString();
			technologyProgressText.color = new Color32(0x8F, 0x7E, 0x60, 0xFF);
			technologyProgressText.fontSize = 14;

			technologyProgressBarImage.fillAmount = percent;
		}

		else
		{
			technologyProgressText.text = new StringBuilder("Not enought resources").ToString();
			technologyProgressText.color = new Color32(0xE5, 0x48, 0x48, 0xFF);
			technologyProgressText.fontSize = 10;

			technologyProgressBarImage.fillAmount = 0;
		}
	}
}
