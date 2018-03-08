using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechTreeManager : MonoBehaviour {

	[Header("Technologies")]
	public ResearchButtonData[] data;

	[Header("Description")]
	public Text technologyTitle;
	public Text technologyDescription;

	[Header("Research progress")]
	public ResearchProgressCanvas researchProgressCanvas;
	public Sprite idleResearchIcon;
	public Image researchIconResourceUI;

	protected TechnologyButton[] _technologyButtonScripts;

	private static TechTreeManager techTreeManager;

	private float pointsToResearch = 50f;

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
				&& data[i] != null)
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
		CanvasGroup cg = GetComponent<CanvasGroup>();
		cg.interactable = true;
		cg.blocksRaycasts = true;
		cg.alpha = 1f;

		CameraController.instance.freeze = true;
		PlayerManager.instance._controllerSelecting.ResetSelection ();
		ControllerUIManager.instance.productionButtonsCanvas.GetComponent<Canvas> ().enabled = false;
		ControllerUIManager.instance.ChangeAndResetView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);
		ShowTechnologyButtonsView ();
	}

	public void Close()
	{
		CanvasGroup cg = GetComponent<CanvasGroup> ();
		cg.interactable = false;
		cg.blocksRaycasts = false;
		cg.alpha = 0f;

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
		researchIconResourceUI.sprite = technologyButtonScript.data.icon;
		researching = true;
		PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).knowledgeGenerated = 0.0f;
		researchProgressCanvas.UpdateProgress (PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).knowledgeGenerated / pointsToResearch);
	}

	void FinishResearch()
	{
		currentResearchButtonScript.Complete ();
		researching = false;
		PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).knowledgeGenerated = 0.0f;
		researchProgressCanvas.icon.sprite = idleResearchIcon;
		researchIconResourceUI.sprite = idleResearchIcon;
		researchProgressCanvas.UpdateProgress(GetResearchedPercentage());
		currentResearchButtonScript.data.executeScript.ActivateTechnology(PlayerManager.myPlayerID);
	}

	void Update () 
	{
		if (researching) 
		{
			PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).knowledgeGenerated += PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).knowledgeGeneration * Time.deltaTime * .1f;

			if (PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).knowledgeGenerated >= pointsToResearch) 
			{
				FinishResearch ();
			}

			researchProgressCanvas.UpdateProgress (PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID).knowledgeGenerated / pointsToResearch);
		}
	}

	public float GetResearchedPercentage()
	{
		return PlayerDataManager.instance.GetPlayerData (PlayerManager.myPlayerID).knowledgeGenerated / pointsToResearch;
	}
}
