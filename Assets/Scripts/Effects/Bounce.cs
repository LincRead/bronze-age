using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour {

    public float bounceTime = 0.25f;
    public float scaleFromMin = 0.75f;

    public void Action()
    {
        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.0f);
        LeanTween.scale(gameObject, new Vector3(scaleFromMin, scaleFromMin, 1.0f), bounceTime);
        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1.0f), bounceTime).setDelay(bounceTime);
    }
}
