using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StarvingPopup : MonoBehaviour {

    public Image bar;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
	
	void Update ()
    {
        bar.fillAmount = 1 - (float)(PlayerDataManager.instance.timeSinceStartedStarving[PlayerManager.myPlayerID] / (float)PlayerDataManager.instance.timeToStartBeforeGameOver);
    }
}
