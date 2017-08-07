using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller views/default")]
public class ControllerUIView : ScriptableObject {

    protected ControllerUIManager ui;

    [HideInInspector]
    public BaseController controller;

    string emptyString = "";

    public virtual void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        this.ui = ui;
        this.controller = controller;

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
        if (controller != null && controller.iconSprite != null)
        {
            ui.icon.enabled = true;
            ui.icon.sprite = controller.iconSprite;
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
