﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class ProductionButton : UnitUIButton {

    public ProductionButtonData data;
    protected GameObject prefab;
    protected BaseController script;

    [HideInInspector]
    public int index = -1;

    protected override void Awake()
    {
        base.Awake();

        if (data != null)
        {
            SetData(data);

            if (data.age > 0)
            {
                EventManager.StartListening("AdvancedCivilizationAge", UpdatedCivAge);
            }
        }
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

        hotkey = data.hotkey;

        UpdateCanBeProduced();
    }

    public void Activate()
    {
		gameObject.SetActive (true);
        _icon.enabled = true;
        UpdateCanBeProduced();
    }

    public void Deactivate()
    {
        if(_icon.enabled)
        {
            _icon.enabled = false;

            if (_button.interactable)
            {
                _button.interactable = false;
            }

            data = null;
            _icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            tooltip = new StringBuilder("?").ToString();
        }
    }

    public void UpdateCanBeProduced()
    {
        if(data == null)
        {
            return;
        }

        // Haven't reached required Age
        if (data.age > PlayerManager.instance.currentAge)
        {
            switch(data.type)
            {
                case PRODUCTION_TYPE.UNIT:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to train " + data.title + ".").ToString();
                    break;

                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to construct " + data.title + ".").ToString();
                    break;
            }

            if (_button.interactable)
            {
                _button.interactable = false;
            }

            _icon.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }

        // Requires technology not yet researched
        else if(!data.requiredTechnology.Equals("") && !Technologies.instance.GetTechnologyCompleted(data.requiredTechnology))
        {
            switch (data.type)
            {
                case PRODUCTION_TYPE.UNIT:
                    tooltip = new StringBuilder("Research " + data.requiredTechnology + " to to train " + data.title + ".").ToString();
                    break;

                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Research " + data.requiredTechnology + " to construct " + data.title + ".").ToString();
                    break;
            }

            if (_button.interactable)
            {
                _button.interactable = false;
            }

            _icon.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }

        else
        {
            tooltip = data.description;

            if (!_button.interactable)
            {
                _button.interactable = true;
            }

            _icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        if (hovered)
        {
            ControllerUIManager.instance.ShowProductionTooltip(data, tooltip);
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
                ControllerUIManager.instance.ShowProductionTooltip(data, tooltip);
            }
        }
    }

    protected override void OnClick()
    {
        base.OnClick();

        if (PlayerManager.instance._controllerSelecting.selectedController != null)
        {
            if(PlayerManager.instance._controllerSelecting.selectedController.controllerType == CONTROLLER_TYPE.BUILDING)
            {
                PlayerManager.instance._controllerSelecting.selectedController.GetComponent<Building>().Produce(index);
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;

        if (!tooltip.Equals("?"))
        {
            ControllerUIManager.instance.ShowProductionTooltip(data, tooltip);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;

        if (!tooltip.Equals("?"))
        {
            ControllerUIManager.instance.HideProductionTooltip();
        }
    }
}


