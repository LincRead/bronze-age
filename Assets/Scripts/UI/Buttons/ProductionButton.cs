using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class ProductionButton : UnitUIButton {

    public ProductionButtonData data;
    protected GameObject prefab;
    protected BaseController script;

    protected override void Awake()
    {
        base.Awake();

        if (data != null)
        {
            SetData(data);
        }
    }

    public void SetData(ProductionButtonData newData)
    {
        data = newData;
        icon.sprite = newData.icon;
        prefab = newData.productionPrefab;
        script = prefab.GetComponent<BaseController>();
        UpdateTooltip();
    }

    void UpdateTooltip()
    {
        // Haven't reached required Age
        if (data.age > PlayerManager.instance.currentAge)
        {
            switch(data.type)
            {
                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to construct " + script.title).ToString();
                    break;
            }
            
            _button.interactable = false;
            icon.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }

        else
        {
            switch (data.type)
            {
                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Construct " + script.title).ToString();
                    break;
            }
                    
        }
    }

    public void UpdatedCivAge()
    {

        if (!_button.interactable && data.age > PlayerManager.instance.currentAge)
        {
            icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            switch (data.type)
            {
                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Construct " + script.title).ToString();
                    break;
            }

        }
    }
}


