using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject bar;
    RectTransform _barTransform;

    void Start()
    {
        _barTransform = bar.GetComponent<RectTransform>();
    }

    public void UpdateHitpointsAmount(int currHP, int maxHP)
    {
        float scale = (float)(currHP / (float)maxHP);
        _barTransform.localScale = new Vector3(scale * 1.0f, 1.0f, 1.0f);
    }
}
