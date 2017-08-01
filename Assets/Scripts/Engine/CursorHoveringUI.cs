using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CursorHoveringUI : MonoBehaviour
{
    bool cursorHoveringUI = true;

    void Start()
    {
        
    }

    void Update()
    {
        ValidateInput();
    }

    void ValidateInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            cursorHoveringUI = true;
        else
            cursorHoveringUI = false;
    }

    public bool IsCursorHoveringUI()
    {
        return cursorHoveringUI;
    }
}
