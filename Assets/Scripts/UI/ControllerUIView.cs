using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller views/default")]
public class ControllerUIView : ScriptableObject {

    protected ControllerUIManager ui;

    [HideInInspector]
    public BaseController _controller;

    string emptyString = "";

    public virtual void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        this.ui = ui;
        this._controller = controller;

        if(_controller)
        {
            ui.title.text = _controller.title;
        }

        else
        {
            ui.title.text = emptyString;
        }

        SetIcon();
    }

    void SetIcon()
    {
        if (_controller != null && _controller.iconSprite != null)
        {
            ui.icon.enabled = true;
            ui.icon.sprite = _controller.iconSprite;

            if(_controller.controllerType == CONTROLLER_TYPE.UNIT)
            {
                ui.icon.rectTransform.sizeDelta = new Vector2(24, 20);
            }

            else
            {
                ui.icon.rectTransform.sizeDelta = new Vector2(32, 32);
            }
            

            if (_controller.playerID > -1)
                ui.icon.material.SetColor("_TeamColor", PlayerDataManager.instance.playerData[_controller.playerID].teamColor);
            else
                ui.icon.material.SetColor("_TeamColor", PlayerDataManager.neutralPlayerColor);
        }
           
        else
        {
            ui.icon.enabled = false;
            ui.icon.sprite = null;
        }
    }

    public virtual void Update()
    {

    }

    public virtual void OnExit()
    {

    }
}
