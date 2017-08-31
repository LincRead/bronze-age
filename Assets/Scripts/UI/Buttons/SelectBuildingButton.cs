using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class SelectBuildingButton : UnitUIButton {

    public BuildBuildingButtonData data;
    protected GameObject buildingPrefab;
    protected Building buildingScript;

    protected override void Awake()
    {
        base.Awake();

        if (data != null)
        {
            SetBuildBuildingData(data);
        }
    }

    public void SetBuildBuildingData(BuildBuildingButtonData buildBuildingData)
    {
        data = buildBuildingData;
        icon.sprite = buildBuildingData.icon;
        buildingPrefab = buildBuildingData.buildingPrefab;
        buildingScript = buildingPrefab.GetComponent<Building>();
        UpdateTooltip();
    }

    void UpdateTooltip()
    {
        // Haven't reached required Age
        if (data.age > PlayerManager.instance.currentAge)
        {
            tooltip = new StringBuilder("Advance your civilization to " + WorldManager.civAgeNames[data.age] + " to construct " + buildingScript.title).ToString();
            _button.interactable = false;
            icon.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }

        else
        {
            tooltip = new StringBuilder("Construct " + buildingScript.title).ToString();
        }
    }

    public void UpdatedCivAge()
    {
        if (!_button.interactable && data.age > PlayerManager.instance.currentAge)
        {
            icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            tooltip = new StringBuilder("Construct " + buildingScript.title).ToString();
        }
    }

    protected override void OnClick()
    {
        base.OnClick();

        // Cancel placement of another building
        PlayerManager.instance.CancelPlaceBuildingState();

        GameObject buildingToPlace = GameObject.Instantiate(buildingPrefab, PlayerManager.mousePosition, Quaternion.identity) as GameObject;
        PlayerManager.instance.SetBuildingPlacementState(buildingToPlace.GetComponent<Building>());
    }
}


