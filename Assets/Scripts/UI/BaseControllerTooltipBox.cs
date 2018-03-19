using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseControllerTooltipBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public string tooltip;

	public Button _button;

	void Start()
	{
		_button = GetComponent<Button>();
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if (_button.interactable && !tooltip.Equals("?"))
		{
			ControllerUIManager.instance.ShowBaseControllerTooltip(tooltip);
		}
	}
		
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!tooltip.Equals("?"))
		{
			ControllerUIManager.instance.HideBaseControllerTooltip();
		}
	}
}
