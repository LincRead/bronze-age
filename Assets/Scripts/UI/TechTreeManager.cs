using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeManager : MonoBehaviour {

	private Canvas _canvas;

	private static TechTreeManager techTreeManager;

	public static TechTreeManager instance
	{
		get
		{
			if (!techTreeManager)
			{
				techTreeManager = FindObjectOfType(typeof(TechTreeManager)) as TechTreeManager;

				if (!techTreeManager)
				{
					Debug.LogError("There needs to be one active TechTreeManager script on a GameObject in the scene.");
				}

				else
				{
					techTreeManager.Init();
				}
			}

			return techTreeManager;
		}
	}

	void Init()
	{
		// ...
	}

	public void Open()
	{
		_canvas.enabled = true;
		//ControllerUIManager.instance._canvas.GetComponent<Canvas>().enabled = false;
		CameraController.instance.freeze = true;
		PlayerManager.instance._controllerSelecting.ResetSelection ();
		ControllerUIManager.instance.ChangeAndResetView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);
	}

	public void Close()
	{
		_canvas.enabled = false;
		//ControllerUIManager.instance._canvas.GetComponent<Canvas>().enabled = true;
		CameraController.instance.freeze = false;
	}

	void Start () 
	{
		_canvas = GetComponent<Canvas> ();
	}

	void Update () 
	{
		
	}
}
