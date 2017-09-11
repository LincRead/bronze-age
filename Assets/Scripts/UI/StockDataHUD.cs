using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class StockDataHUD : MonoBehaviour {

    PlayerData myPlayerData;

    public Text housing;
    public Text foodProduction;
    public Text foodStock;
    public Text timber;
    public Text stone;

    private void Awake()
    {
        myPlayerData = PlayerDataManager.instance.playerData[PlayerManager.myPlayerID];
    }

    private void Start()
    {
        UpdateHousing();
        UpdateTimber();
        UpdateStoneTools();
    }

    private void OnEnable()
    {
        EventManager.StartListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StartListening("UpdateFoodProductionUI", UpdateFoodProduction);
        EventManager.StartListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StartListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StartListening("UpdateStoneStockUI", UpdateStoneTools);
    }

    private void OnDisable()
    {
        EventManager.StopListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StopListening("UpdateFoodProductionUI", UpdateFoodProduction);
        EventManager.StopListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StopListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StopListening("UpdateStoneStockUI", UpdateStoneTools);
    }

    void UpdateHousing()
    {
        housing.text = new StringBuilder(myPlayerData.housing.ToString() + "/" + myPlayerData.population.ToString()).ToString();
    }

    void UpdateFoodProduction()
    {
        foodProduction.text = myPlayerData.foodProduction.ToString();
    }

    void UpdateFoodStock()
    {
        foodStock.text = myPlayerData.food.ToString();
    }

    void UpdateTimber()
    {
        timber.text = myPlayerData.timber.ToString();
    }

    void UpdateStoneTools()
    {
        stone.text = myPlayerData.stoneTools.ToString();
    }
}
