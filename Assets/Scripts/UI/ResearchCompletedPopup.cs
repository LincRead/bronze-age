using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchCompletedPopup : MonoBehaviour {

    public Image researchIcon;
    public Text researchTitle;

    public float showForSecs = 6;
    private float showingForSecs = 0;

    bool showing = false;

    private void Update()
    {
        if(showing)
        {
            showingForSecs += Time.deltaTime;

            if(showingForSecs >= showForSecs)
            {
                Close();
            }
        }
    }

    public void Show(Sprite icon, string researchTitle4)
    {
        researchIcon.sprite = icon;
        researchTitle.text = researchTitle4;

        gameObject.SetActive(true);

        showing = true;
        showingForSecs = 0.0f;

        RectTransform rect = researchIcon.GetComponent<RectTransform>();
        rect.localScale = new Vector3(0.85f, 0.85f, 1.0f);
        LeanTween.scale(rect, new Vector3(1.15f, 1.15f, 1.0f), .75f).setLoopPingPong();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        showing = false;
    }
}
