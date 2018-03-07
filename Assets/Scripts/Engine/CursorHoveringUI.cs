using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CursorHoveringUI : MonoBehaviour
{
    public static bool value = true;

    void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject ()) {
			value = true;
		} else {
			value = false;
		}
	}
}
