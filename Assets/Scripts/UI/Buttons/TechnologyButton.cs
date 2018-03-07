using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class TechnologyButton : UnitUIButton {

	public ProductionButtonData data;

	public enum TECH_BTN_STATE
	{
		NOT_RESEARCHED,
		RESEARCHING,
		RESEARCHED
	}

	[HideInInspector]
	public TECH_BTN_STATE technologyButtonState = TECH_BTN_STATE.NOT_RESEARCHED;

	[HideInInspector]
	public int index = -1;

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

		if(data.icon != null)
		{
			_icon.sprite = data.icon;
		}
	}

	public void Activate()
	{
		_icon.enabled = true;

		_icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
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
				
			_icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}

	protected override void OnClick()
	{
		switch (technologyButtonState) 
		{
			case TECH_BTN_STATE.NOT_RESEARCHED: // Tech
			{
				// Start research

			}

			break;

			case TECH_BTN_STATE.RESEARCHING: // Tech
			{
				// Cancel research

			}
			break;

			case TECH_BTN_STATE.RESEARCHED:
			{
				// Nothing happens

			}
			break;
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		hovered = true;


	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		hovered = false;


	}

}
