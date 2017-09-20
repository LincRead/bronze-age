using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class StockDataHUD : MonoBehaviour {

    PlayerData myPlayerData;

    public Text population;
    public Text housing;
    public Text foodStock;
    public Text foodIntake;
    public Text timber;
    public Text metal;
    public Text prosperity;

    private void Awake()
    {
        myPlayerData = PlayerDataManager.instance.playerData[PlayerManager.myPlayerID];
    }

    private void Start()
    {
        UpdatePopulation();
        UpdateTimber();
        UpdateMetal();
        UpdateFoodStock();
        UpdateFoodIntake();
        UpdateHousing();
        UpdateProsperity();
    }

    private void OnEnable()
    {
        EventManager.StartListening("UpdatePopulationStockUI", UpdatePopulation);
        EventManager.StartListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StartListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StartListening("UpdateFoodIntakeUI", UpdateFoodIntake);
        EventManager.StartListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StartListening("UpdateMetalStockUI", UpdateMetal);
        EventManager.StartListening("UpdateProsperityStockUI", UpdateProsperity);
    }

    private void OnDisable()
    {
        EventManager.StopListening("UpdatePopulationStockUI", UpdatePopulation);
        EventManager.StopListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StopListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StopListening("UpdateFoodIntakeUI", UpdateFoodIntake);
        EventManager.StopListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StopListening("UpdateMetalStockUI", UpdateMetal);
        EventManager.StopListening("UpdateProsperityStockUI", UpdateProsperity);
    }

    void UpdatePopulation()
    {
        population.text = new StringBuilder(myPlayerData.newCitizens.ToString()).ToString();
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
        foodStock.text = ((int)myPlayerData.foodStock).ToString();
    }

    void UpdateTimber()
    {
        timber.text = myPlayerData.timber.ToString();
    }

    void UpdateMetal()
    {
        metal.text = myPlayerData.metal.ToString();
    }

    void UpdateProsperity()
    {
        int prosperityNumber = myPlayerData.staticProsperity;

        if (myPlayerData.foodStock < 0)
        {
            prosperityNumber += (int)(myPlayerData.foodStock / 20);
        }

        else
        {
            prosperityNumber++;
            prosperityNumber += (int)(myPlayerData.foodStock / 50);
        }

        if(myPlayerData.housing >= myPlayerData.population)
        {
            prosperityNumber++;
        }

        else
        {
            prosperityNumber -= 3;
        }

        if(myPlayerData.age >= 2)
        {
            prosperityNumber -= (int)(myPlayerData.population / 10);
        }

        if(prosperityNumber > -1)
        {
            prosperity.text = new StringBuilder("+" + prosperityNumber.ToString()).ToString();
        }

        else
        {
            prosperity.text = prosperityNumber.ToString();
        }
    }
}
