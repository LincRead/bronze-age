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

        EventManager.StartListening("AdvancedCivilizationAge", UpdatedCivAge);
    }

    public void SetData(ProductionButtonData newData)
    {
        data = newData;

        if(data.icon != null)
        {
            _icon.sprite = data.icon;
        }
        
        prefab = data.productionPrefab;

        if(prefab != null)
        {
            script = prefab.GetComponent<BaseController>();
        }
        
        UpdateTooltip();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void UpdateTooltip()
    {
        // Haven't reached required Age
        if (data.age > PlayerManager.instance.currentAge)
        {
            switch(data.type)
            {
                case PRODUCTION_TYPE.UNIT:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to train " + script.title).ToString();
                    break;

                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to construct " + script.title).ToString();
                    break;

                case PRODUCTION_TYPE.TECHNOLOGY:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to " + data.title).ToString();
                    break;
            }
            
            _button.interactable = false;
            _icon.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }

        else
        {
            switch (data.type)
            {
                case PRODUCTION_TYPE.UNIT:
                    tooltip = new StringBuilder("Train " + script.title).ToString();
                    break;

                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Construct " + data.title).ToString();
                    break;

                case PRODUCTION_TYPE.TECHNOLOGY:
                    tooltip = data.tooltip;
                    break;
            }
                    
        }
    }

    public void UpdatedCivAge()
    {
        if (!_button.interactable && data.age == PlayerManager.instance.currentAge)
        {
            EventManager.StopListening("AdvancedCivilizationAge", UpdatedCivAge);
            _icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            _button.interactable = true;
            UpdateTooltip();

            if(hovered)
            {
                ControllerUIManager.instance.ShowTooltip(tooltip);
            }
        }
    }

    protected override void OnClick()
    {
        base.OnClick();

        if (PlayerManager.instance._controllerSelecting.selectedController != null
            && PlayerManager.instance._controllerSelecting.selectedController.controllerType == CONTROLLER_TYPE.BUILDING)
        {
            PlayerManager.instance._controllerSelecting.selectedController.GetComponent<Building>().Produce(data.index);
        }
    }
}


