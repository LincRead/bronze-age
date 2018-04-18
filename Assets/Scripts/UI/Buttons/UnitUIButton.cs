using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string title = "?";
    public KeyCode hotkey;
    public Image _icon;

    protected Button _button;

    [HideInInspector]
    public string tooltip;

    protected bool hovered = false;

	[Header("Tooltip settings")]
	public bool actionTooltip = false;
	public bool baseControllerTooltip = false;

    protected virtual void Awake()
    {
        // Todo make tooltip system
        tooltip = title;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    void Update()
    {
        // Todo, use event in inspector instead
        if (_button != null 
            && _button.enabled
            && _button.interactable
            && Input.GetKeyDown(hotkey))
        {
            OnClick();
        }
    }
	
	protected virtual void OnClick()
    {
        EventManager.TriggerEvent("SetDefaultCursor");
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {      
        hovered = true;

        ShowTooltip();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;

        HideTooltip();
    }

    protected virtual void ShowTooltip()
    {
		if (!_button.interactable) 
		{
			return;
		}

		if (actionTooltip) 
		{
			ControllerUIManager.instance.ShowActionTooltip(tooltip);
		} 

		else if(baseControllerTooltip)
		{
			ControllerUIManager.instance.ShowBaseControllerTooltip(tooltip);
		}
        
    }

    protected virtual void HideTooltip()
    {
		if (actionTooltip) 
		{
			ControllerUIManager.instance.HideActionTooltip();
		} 

		else if(baseControllerTooltip)
		{
			ControllerUIManager.instance.HideBaseControllerTooltip();
		}
    }
}