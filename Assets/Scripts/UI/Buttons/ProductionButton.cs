﻿using UnityEngine;
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

        UpdateCanBeProduced();
    }

    public void Activate()
    {
        _icon.enabled = true;
        _button.interactable = true;
        UpdateCanBeProduced();
    }

    public void Deactivate()
    {
        _icon.enabled = false;
        _button.interactable = false;
        _icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void UpdateCanBeProduced()
    {
        // Haven't reached required Age
        if (data.age > PlayerManager.instance.currentAge)
        {
            switch(data.type)
            {
                case PRODUCTION_TYPE.UNIT:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to train " + data.title).ToString();
                    break;

                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to construct " + data.title).ToString();
                    break;

                case PRODUCTION_TYPE.TECHNOLOGY:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to research " + data.title).ToString();
                    break;
            }
            
            _button.interactable = false;
            _icon.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }

        // Requires technology not yet researched
        else if(!data.requiredTechnology.Equals("") && !Technologies.instance.GetTechnologyCompleted(data.requiredTechnology))
        {
            switch (data.type)
            {
                case PRODUCTION_TYPE.UNIT:
                    tooltip = new StringBuilder("Research " + data.requiredTechnology + " to to train " + data.title).ToString();
                    break;

                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Research " + data.requiredTechnology + " to construct " + data.title).ToString();
                    break;

                case PRODUCTION_TYPE.TECHNOLOGY:
                    tooltip = new StringBuilder("Research " + data.requiredTechnology + " to research " + data.title).ToString();
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
                    tooltip = new StringBuilder("Train " + data.title).ToString();
                    break;

                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Construct " + data.title).ToString();
                    break;

                case PRODUCTION_TYPE.TECHNOLOGY:
                    tooltip = new StringBuilder("Research " + data.title).ToString();
                    break;
            }

            _button.interactable = true;
            _icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        if (hovered)
        {
            ControllerUIManager.instance.ShowTooltip(tooltip);
        }
    }

    public void UpdatedCivAge()
    {
        if (!_button.interactable && data.age == PlayerManager.instance.currentAge)
        {
            EventManager.StopListening("AdvancedCivilizationAge", UpdatedCivAge);
            _icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            _button.interactable = true;
            UpdateCanBeProduced();

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


