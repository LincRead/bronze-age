using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class StockDataHUD : MonoBehaviour {

    PlayerData myPlayerData;
    public Image foodSurplusBar;
    public Text foodBonusText;

    public Text population;
    public Text housing;
    public Text foodStock;
    public Text foodIntake;
    public Text timber;
    public Text wealth;
    public Text metal;

    private void Awake()
    {
        myPlayerData = PlayerDataManager.instance.playerData[PlayerManager.myPlayerID];
    }

    private void Start()
    {
        UpdateTimber();
        UpdateWealth();
        UpdateMetal();
        UpdateFoodStock();
        UpdateFoodIntake();
        UpdateHousing();
    }

    private void OnEnable()
    {
        EventManager.StartListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StartListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StartListening("UpdateFoodIntakeUI", UpdateFoodIntake);
        EventManager.StartListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StartListening("UpdateWealthStockUI", UpdateWealth);
        EventManager.StartListening("UpdateMetalStockUI", UpdateMetal);
    }

    private void OnDisable()
    {
        EventManager.StopListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StopListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StopListening("UpdateFoodIntakeUI", UpdateFoodIntake);
        EventManager.StopListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StopListening("UpdateWealthStockUI", UpdateWealth);
        EventManager.StopListening("UpdateMetalStockUI", UpdateMetal);
    }

    void UpdateHousing()
    {
        housing.text = new StringBuilder(myPlayerData.population.ToString() + "/" + myPlayerData.housing.ToString()).ToString();
    }

    void UpdateFoodIntake()
    {
        if (myPlayerData.foodIntake > -1)
        {
            foodIntake.text = new StringBuilder("+" + myPlayerData.foodIntake.ToString()).ToString();
        }

        else
        {
            foodIntake.text = new StringBuilder(myPlayerData.foodIntake.ToString()).ToString();
        }
    }

    void UpdateFoodStock()
    {
        foodStock.text =((int)(myPlayerData.foodInStock)).ToString();
        foodSurplusBar.GetComponent<FoodSurplusBar>().UpdateBar(myPlayerData.foodInStock);
        foodBonusText.GetComponent<FoodBonusText>().UpdateBonusText(myPlayerData.foodSurplusLevel);
    }

    void UpdateTimber()
    {
        timber.text = myPlayerData.timber.ToString();
    }

    void UpdateWealth()
    {
        wealth.text = myPlayerData.wealth.ToString();
    }

    void UpdateMetal()
    {
        metal.text = myPlayerData.metal.ToString();
    }
}
