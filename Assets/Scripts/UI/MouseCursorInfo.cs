using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseCursorInfo: MonoBehaviour {

    public Text text;
    public Image image;

	void Start ()
    {
		
	}
	
	void LateUpdate ()
    {
        transform.position = PlayerManager.mousePosition;
	}

    public void UpdateValue(int value)
    {
        gameObject.SetActive(true);
        text.text = value.ToString();
    }
}
