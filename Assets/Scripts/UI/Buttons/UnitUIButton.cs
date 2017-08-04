using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string title = "?";
    public KeyCode hotkey;

    Button _button;
    protected string tooltip;

    protected virtual void Start () {

        // Todo make tooltip system
        tooltip = title;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (_button != null 
            && _button.enabled
            && Input.GetKeyDown(hotkey))
            OnClick();
    }
	
	protected virtual void OnClick() {
	
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        ControllerUIManager.instance.ShowTooltip(tooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ControllerUIManager.instance.HideTooltip();
    }
}
