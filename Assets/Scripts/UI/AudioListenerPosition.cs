using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerPosition : MonoBehaviour {

    public Camera parentCamera;
    Transform _transform;

	void Start ()
    {
        _transform = GetComponent<Transform>();

    }
	
	void Update ()
    {
        _transform.position = new Vector3(
            CameraController.instance._transform.position.x, 
            CameraController.instance._transform.position.y, 
            CameraController.instance._transform.position.y);
    }
}
