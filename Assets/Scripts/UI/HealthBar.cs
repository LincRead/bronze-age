using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public GameObject bar;
    SpriteRenderer _barSpriteRenderer;

    public GameObject outline;
    SpriteRenderer _outlineSpriteRenderer;

    int maxHp = 0;
    int currHp = 0;

    void Start ()
    {
        _barSpriteRenderer = bar.GetComponent<SpriteRenderer>();
        _outlineSpriteRenderer = outline.GetComponent<SpriteRenderer>();
	}
	
    public void Activate()
    {
        _barSpriteRenderer.enabled = true;
        _outlineSpriteRenderer.enabled = true;
    }

    public void Disable()
    {
        _barSpriteRenderer.enabled = false;
        _outlineSpriteRenderer.enabled = false;
    }

    /*public UpdateSprites()
    {

    }*/
}
