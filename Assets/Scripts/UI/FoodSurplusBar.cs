using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodSurplusBar : MonoBehaviour {

    public Sprite[] foodSurplusSprites;
    Image _image;
    float fillPercent = 0;
    PlayerData myPlayerData;

    private void Awake()
    {
        _image = GetComponent<Image>();
        myPlayerData = PlayerDataManager.instance.playerData[PlayerManager.myPlayerID];
    }

    public void UpdateBar(float foodInStock)
    {
        fillPercent = (PlayerDataManager.foodPerSurplusLevel + foodInStock) / (PlayerDataManager.foodPerSurplusLevel * 5);
        if (myPlayerData.foodSurplusLevel == 0)
        {
            _image.sprite = foodSurplusSprites[0];
        }

        else if (myPlayerData.foodSurplusLevel == 1)
        {
            _image.sprite = foodSurplusSprites[1];
        }

        else
        {
            _image.sprite = foodSurplusSprites[2];
        }

        if (fillPercent > 1)
        {
            fillPercent = 1;
        }

        _image.fillAmount = fillPercent;
    }
}
