using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeManager : MonoBehaviour {

	private Canvas _canvas;

	[Header("Technologies")]
	public ProductionButtonData[] data;

	protected TechnologyButton[] _technologyButtonScripts;

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


	void Start () 
	{
		_canvas = GetComponent<Canvas> ();
	}

	void Init()
	{
		// ...
	}

	public void SetupTechnologyButtonsView(TechnologyButton[] buttons)
	{
		_technologyButtonScripts = buttons;
	}

	public void ShowTechnologyButtonsView()
	{
		bool[] buttonWithIndexesActivated = new bool[_technologyButtonScripts.Length];
		for (int i = 0; i < buttonWithIndexesActivated.Length; i++)
		{
			buttonWithIndexesActivated[i] = false;
		}

		for (int i = 0; i < data.Length; i++)
		{
			if (data != null
				&& i < data.Length
				&& data[i] != null
				&& data[i].type == PRODUCTION_TYPE.TECHNOLOGY)
			{
				// Todo make condition for required technology
				_technologyButtonScripts[i].index = i;
				_technologyButtonScripts[i].SetData(data[i]);
				_technologyButtonScripts[i].Activate();
				buttonWithIndexesActivated[i] = true;
			}
		}

		// Deactivate buttons that didn't get activated
		for (int i = 0; i < buttonWithIndexesActivated.Length; i++)
		{
			if(!buttonWithIndexesActivated[i])
			{
				_technologyButtonScripts[i].Deactivate();
			}
		}
	}

	public void Open()
	{
		_canvas.enabled = true;
		CameraController.instance.freeze = true;
		PlayerManager.instance._controllerSelecting.ResetSelection ();
		ControllerUIManager.instance.productionButtonsCanvas.GetComponent<Canvas> ().enabled = false;
		ControllerUIManager.instance.ChangeAndResetView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);
		ShowTechnologyButtonsView ();
	}

	public void Close()
	{
		_canvas.enabled = false;
		ControllerUIManager.instance.productionButtonsCanvas.GetComponent<Canvas> ().enabled = true;
		CameraController.instance.freeze = false;
	}

	void Update () 
	{
		
	}
}
