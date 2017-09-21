using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ResourceTooltipBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltip;

    bool hovered = false;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;

        if (!tooltip.Equals("?"))
        {
            ControllerUIManager.instance.ShowResourceTooltip(tooltip);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;

        if (!tooltip.Equals("?"))
        {
            ControllerUIManager.instance.HideResourceTooltip();
        }
    }
}
