using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public GameObject bar;
    SpriteRenderer _barSpriteRenderer;
    Transform _barTransform;

    public GameObject outline;
    SpriteRenderer _outlineSpriteRenderer;

    float width = 0.0f;

    void Start ()
    {
        _barSpriteRenderer = bar.GetComponent<SpriteRenderer>();
        _barTransform = bar.GetComponent<Transform>();
        width = _barSpriteRenderer.bounds.size.x;

        _outlineSpriteRenderer = outline.GetComponent<SpriteRenderer>();

        Deactivate();
	}

    public void SetAlignment(bool friendlyUnit)
    {
        if(friendlyUnit)
            _barSpriteRenderer.color = Color.green;
        else
            _barSpriteRenderer.color = Color.red;
    }
	
    public void Activate()
    {
        _barSpriteRenderer.enabled = true;
        _outlineSpriteRenderer.enabled = true;
    }

    public void Deactivate()
    {
        _barSpriteRenderer.enabled = false;
        _outlineSpriteRenderer.enabled = false;
    }

    public void UpdateHitpointsAmount(int currHP, int maxHP)
    {
        float scale = (float)(currHP / (float)maxHP);
        _barTransform.localScale = new Vector3(scale * 1.0f, 1.0f, 1.0f);
    }
}
