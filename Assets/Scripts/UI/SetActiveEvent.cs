using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveEvent : MonoBehaviour
{
    public string activateEventName;
    public string disableEventName;

    private void Start()
    {
        Disable();
    }

    private void OnEnable()
    {
        EventManager.StartListening(this.activateEventName, Activate);
        EventManager.StartListening(this.disableEventName, Disable);
    }

    private void OnDisable()
    {
        EventManager.StopListening(this.activateEventName, Activate);
        EventManager.StopListening(this.disableEventName, Disable);
    }

    private void Activate()
    {
        for (var i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void Disable()
    {
        for (var i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
