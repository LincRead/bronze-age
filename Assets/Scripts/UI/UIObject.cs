using UnityEngine;
using System.Collections;

public class UIObject : MonoBehaviour {

    Transform _transform;
    Vector3 guiPos;

	// Use this for initialization
	void Start () {
        _transform = GetComponent<Transform>();
        guiPos = _transform.position;   
	}
	
	// Update is called once per frame
	void Update () {
        _transform.position = guiPos + CameraController.Manager._myTransform.position;
	}
}
