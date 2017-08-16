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

        if(controller)
        {
            ui.title.text = controller.title;
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
