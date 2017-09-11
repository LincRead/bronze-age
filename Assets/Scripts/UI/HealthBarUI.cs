using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class HealthBarUI : MonoBehaviour
{
    public Image hitpointsBar;
    public Text hitpointsText;

    RectTransform _barTransform;

    private void Awake()
    {
        _barTransform = hitpointsBar.GetComponent<RectTransform>();
    }

    public void ShowHitpoints(int hp, int max)
    {
        gameObject.SetActive(true);
        UpdateHitpoints(hp, max);
    }

    public void HideHitpoints()
    {
        gameObject.SetActive(false);
    }

    public void UpdateHitpoints(int hp, int max)
    {
        hitpointsText.text = new StringBuilder(hp.ToString() + "/" + max.ToString()).ToString();
        UpdateHitpointsAmount(hp, max);
    }

    public void UpdateHitpointsAmount(int currHP, int maxHP)
    {
        float scale = (float)(currHP / (float)maxHP);
        _barTransform.localScale = new Vector3(scale * 1.0f, 1.0f, 1.0f);
    }
}
