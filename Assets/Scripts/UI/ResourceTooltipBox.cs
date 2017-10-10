using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ResourceTooltipBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltip;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!tooltip.Equals("?"))
        {
            ControllerUIManager.instance.ShowResourceTooltip(tooltip);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!tooltip.Equals("?"))
        {
            ControllerUIManager.instance.HideResourceTooltip();
        }
    }
}
