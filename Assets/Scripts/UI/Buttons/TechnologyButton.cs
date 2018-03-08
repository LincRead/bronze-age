using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class TechnologyButton : UnitUIButton {

	public ResearchButtonData data;

	Image _image;

	[Header("Sprite for states")]
	private Sprite defaultSprite;
	public Sprite researchingSprite;
	public Sprite researchedSprite;

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
			
		_image = GetComponent<Image> ();
		defaultSprite = _image.sprite;
	}

	public void SetData(ResearchButtonData newData)
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
			if (_button.interactable)
			{
				_button.interactable = false;
			}

			_icon.enabled = false;
		}
	}

	protected override void OnClick()
	{
		switch (technologyButtonState) 
		{
			case TECH_BTN_STATE.NOT_RESEARCHED: // Tech
			{
				// Start research
				technologyButtonState = TECH_BTN_STATE.RESEARCHING;
				_image.sprite = researchingSprite;

				TechTreeManager.instance.StartResearch (this);
			}
				
			break;
		}
	}

	public void Cancel()
	{
		technologyButtonState = TECH_BTN_STATE.NOT_RESEARCHED;
		_image.sprite = defaultSprite;
	}

	public void Complete()
	{
		technologyButtonState = TECH_BTN_STATE.RESEARCHED;
		_image.sprite = researchedSprite;

		Deactivate ();

		var newDisabledColor = _button.colors;
		newDisabledColor.disabledColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		_button.colors = newDisabledColor;
		_icon.enabled = true;
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		hovered = true;

		if (data != null) 
		{
			TechTreeManager.instance.technologyDescription.text = data.description;
			TechTreeManager.instance.technologyTitle.text = data.title;
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		hovered = false;

		if (data != null)
		{
			TechTreeManager.instance.technologyDescription.text = "";
			TechTreeManager.instance.technologyTitle.text = "";
		}
	}
}
