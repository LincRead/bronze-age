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
    public Text prosperity;

    private void Awake()
    {
        myPlayerData = PlayerDataManager.instance.playerData[PlayerManager.myPlayerID];
    }

    private void Start()
    {
        UpdatePopulation();
        UpdateTimber();
        UpdateWealth();
        UpdateMetal();
        UpdateFoodStock();
        UpdateFoodIntake();
        UpdateHousing();
        UpdateProsperity();
    }

    private void OnEnable()
    {
        EventManager.StartListening("UpdateNewCitizensStockUI", UpdatePopulation);
        EventManager.StartListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StartListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StartListening("UpdateFoodIntakeUI", UpdateFoodIntake);
        EventManager.StartListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StartListening("UpdateWealthStockUI", UpdateWealth);
        EventManager.StartListening("UpdateMetalStockUI", UpdateMetal);
        EventManager.StartListening("UpdateProsperityStockUI", UpdateProsperity);
    }

    private void OnDisable()
    {
        EventManager.StopListening("UpdateNewCitizensStockUI", UpdatePopulation);
        EventManager.StopListening("UpdateHousingStockUI", UpdateHousing);
        EventManager.StopListening("UpdateFoodStockUI", UpdateFoodStock);
        EventManager.StopListening("UpdateFoodIntakeUI", UpdateFoodIntake);
        EventManager.StopListening("UpdateTimberStockUI", UpdateTimber);
        EventManager.StopListening("UpdateWealthStockUI", UpdateWealth);
        EventManager.StopListening("UpdateMetalStockUI", UpdateMetal);
        EventManager.StopListening("UpdateProsperityStockUI", UpdateProsperity);
    }

    void UpdatePopulation()
    {
        //population.text = new StringBuilder(myPlayerData.newCitizens.ToString()).ToString();
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

    void UpdateProsperity()
    {
        /*int prosperityNumber = myPlayerData.staticProsperity;

        // Food stock
        if (myPlayerData.foodStock < 0)
        {
            prosperityNumber += (int)(myPlayerData.foodStock / 20);
        }

        else
        {
            prosperityNumber++;

            if(myPlayerData.foodStock > 500) prosperityNumber++;
            if (myPlayerData.foodStock > 250) prosperityNumber++;
            if (myPlayerData.foodStock > 100) prosperityNumber++;
            if (myPlayerData.foodStock > 50) prosperityNumber++;
        }

        // Housing
        if(myPlayerData.housing >= myPlayerData.population)
        {
            prosperityNumber++;
        }

        else
        {
            prosperityNumber -= 3;
        }

        // Population - decease
        prosperityNumber -= (int)(myPlayerData.population / 10);

        SetProsperityText(prosperityNumber);
        myPlayerData.realProsperity = prosperityNumber;*/
    }

    void SetProsperityText(int value)
    {
        return;

        // Set text
        if (value > -1)
        {
            prosperity.text = new StringBuilder("+" + value.ToString()).ToString();
        }

        else
        {
            prosperity.text = value.ToString();
        }
    }
}
