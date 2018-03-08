using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Research Button Data")]
public class ResearchButtonData : ScriptableObject {

	[Header("Title")]
	public string title;

	[Header("Description")]
	public string description;

	[Header("Execute when production finishes")]
	public FinishedResearchAction executeScript;

	[Header("Icon")]
	public Sprite icon;

	[Header("Required resources")]
	public int food = 0;
	public int timber = 0;
	public int wealth = 0;
	public int metal = 0;
}
