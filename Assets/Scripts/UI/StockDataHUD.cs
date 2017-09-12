using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class StockDataHUD : MonoBehaviour {

    PlayerData myPlayerData;

    public Text housing;
    public Text foodStock;
    public Text foodIntake;
    public Text timber;
    public Text metal;

    private void Awake()
    {
        myPlayerData = PlayerDataManager.instance.playerData[PlayerManager.myPlayerID];
    }

    private void Start()
    {
        UpdateHousing();
        UpdateTimber();
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
        EventManager.StartListening("UpdateMetalStockUI", UpdateMetal);
    }

    private void OnDisable()
    {
        EventManager.StopListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StopListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StopListening("UpdateFoodIntakeUI", UpdateFoodIntake);
        EventManager.StopListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StopListening("UpdateMetalStockUI", UpdateMetal);
    }

    void UpdateHousing()
    {
        housing.text = new StringBuilder(myPlayerData.housing.ToString() + "/" + myPlayerData.population.ToString()).ToString();
    }

    void UpdateFoodIntake()
    {
        foodIntake.text = myPlayerData.foodIntake.ToString();
    }

    void UpdateFoodStock()
    {
        foodStock.text = myPlayerData.foodStock.ToString();
    }

    void UpdateTimber()
    {
        timber.text = myPlayerData.timber.ToString();
    }

    void UpdateMetal()
    {
        metal.text = myPlayerData.metal.ToString();
    }
}
