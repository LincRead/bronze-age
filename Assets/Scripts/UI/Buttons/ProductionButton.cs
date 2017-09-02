using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

[CreateAssetMenu(menuName = "UI/Production Button")]
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
        Debug.Log(newData);

        data = newData;

        if(newData.icon != null)
        {
            _icon.sprite = newData.icon;
        }
        
        prefab = newData.productionPrefab;

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
                case PRODUCTION_TYPE.BUILDING:
                    tooltip = new StringBuilder("Construct " + script.title).ToString();
                    break;

                case PRODUCTION_TYPE.TECHNOLOGY:
                    tooltip = data.tooltip;
                    break;
            }
                    
        }
    }

    public void UpdatedCivAge()
    {
        if (!_button.interactable && data.age > PlayerManager.instance.currentAge)
        {
            _icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            UpdateTooltip();
        }
    }
}


