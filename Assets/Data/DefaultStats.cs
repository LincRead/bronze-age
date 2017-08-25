using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStats : ScriptableObject
{
    [Header("Selection")]
    public GameObject selectionCircle;

    [Header("Title")]
    public string title = "Unit";

    [Header("Type")]
    public CONTROLLER_TYPE controllerType;

    [Header("Default stats")]
    public int size = 1;

    [Header("UI")]
    public Sprite iconSprite;
    public Sprite[] statSprites = new Sprite[4];
}
