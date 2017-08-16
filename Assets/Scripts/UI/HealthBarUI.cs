using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject bar;
    Image _barImage;
    RectTransform _barTransform;

    float width = 0.0f;

    void Start()
    {
        _barImage = bar.GetComponent<Image>();
        _barTransform = bar.GetComponent<RectTransform>();
        width = _barTransform.rect.width;
    }

    public void UpdateHitpointsAmount(int currHP, int maxHP)
    {
        float scale = (float)(currHP / (float)maxHP);
        _barTransform.localScale = new Vector3(scale * 1.0f, 1.0f, 1.0f);
    }
}
