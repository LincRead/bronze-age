using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechTreeManager : MonoBehaviour {

	private Canvas _canvas;

	[Header("Technologies")]
	public ProductionButtonData[] data;

	[Header("Description")]
	public Text technologyTitle;
	public Text technologyDescription;

	[Header("Research progress")]
	public ResearchProgressCanvas researchProgressCanvas;
	public Sprite idleResearchIcon;

	protected TechnologyButton[] _technologyButtonScripts;

	private static TechTreeManager techTreeManager;

	float pointsResearched = 0.0f;
	float pointsToResearch = 25f;

	bool researching = false;

	private TechnologyButton currentResearchButtonScript = null;

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
	
		technologyDescription.text = "";
		technologyTitle.text = "";
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

	public void StartResearch(TechnologyButton technologyButtonScript)
	{
		if (researching) 
		{
			currentResearchButtonScript.Cancel ();
		}

		currentResearchButtonScript = technologyButtonScript;
		researchProgressCanvas.icon.sprite = technologyButtonScript.data.icon;
		researching = true;
		pointsResearched = 0.0f;
	}

	void FinishResearch()
	{
		currentResearchButtonScript.Complete ();
		researching = false;
		pointsResearched = 0.0f;
		researchProgressCanvas.icon.sprite = idleResearchIcon;
	}

	void Update () 
	{
		if (researching) 
		{
			pointsResearched += PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).knowledgeGeneration * Time.deltaTime;

			if (pointsResearched >= pointsToResearch) 
			{
				FinishResearch ();
			}
		}

		researchProgressCanvas.UpdateProgress (pointsResearched / pointsToResearch);
	}
}
