using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public GameObject bar;
    SpriteRenderer _barSpriteRenderer;
    Transform _barTransform;

    public GameObject outline;
    SpriteRenderer _outlineSpriteRenderer;

    float scale = 0.0f;
    float width = 0.0f;

    public void Init (int size)
    {
        gameObject.transform.position += new Vector3(0.0f, 0.12f + (size * 0.16f)); 
        _barSpriteRenderer = bar.GetComponent<SpriteRenderer>();
        _barTransform = bar.GetComponent<Transform>();
        scale = size;
        width = _barSpriteRenderer.bounds.size.x;

        _outlineSpriteRenderer = outline.GetComponent<SpriteRenderer>();
        outline.GetComponent<Transform>().localScale = new Vector3(scale, 1.0f);

        Deactivate();
	}

    public void SetAlignment(bool friendlyUnit)
    {
        if(friendlyUnit)
        {
            _barSpriteRenderer.color = Color.green;
        }
            
        else
        {
            _barSpriteRenderer.color = Color.red;
        }
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
        float scaleX = (float)(currHP / (float)maxHP);
        _barTransform.localScale = new Vector3(scaleX * scale,  1.0f, 1.0f);

        _barTransform.localPosition = new Vector3(
        - (((width / 2) * ((1 - scaleX)))) * scale,
        0.0f,
        0.0f);
    }
}
