using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class FoodBonusText : MonoBehaviour {

    Text _text;
    string txt = "";

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    // Update is called once per frame
    public void UpdateBonusText(int foodLevel)
    {
        // Todo: base on div
        switch(foodLevel)
        {
            case 0: txt = new StringBuilder("-50% production speed").ToString(); break;
            case 1: txt = new StringBuilder("").ToString(); break;
            case 2: txt = new StringBuilder("+10% unit training speed").ToString(); break;
            case 3: txt = new StringBuilder("+20% unit training speed").ToString(); break;
            case 4: txt = new StringBuilder("+30% unit training speed").ToString(); break;
        }

        _text.text = txt;
    }
}
