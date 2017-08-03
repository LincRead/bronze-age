using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    Button btn;
    public string title = "?";
    public KeyCode hotkey;
    protected string tooltip;

    protected virtual void Start () {
        tooltip = title;
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (btn != null && btn.enabled && Input.GetKeyDown(hotkey))
            OnClick();
    }
	
	protected virtual void OnClick() {
	
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        UnitUIManager.instance.ShowTooltip(tooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnitUIManager.instance.HideTooltip();
    }
}
