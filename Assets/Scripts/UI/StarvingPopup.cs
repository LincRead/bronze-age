using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StarvingPopup : MonoBehaviour {

    public Image bar;

    float scale = 0.0f;
    float width = 0.0f;

    void Start ()
    {

    }

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
        UpdateBar();

        bar.fillAmount = 1 - (float)(PlayerDataManager.instance.timeSinceStartedStarving[PlayerManager.myPlayerID] / (float)PlayerDataManager.instance.timeToStartBeforeGameOver);
    }

    public void UpdateBar()
    {
        
    }
}
