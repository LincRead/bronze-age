using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTooltip : MonoBehaviour {

    public Text title;

    public void UpdateData(string desc)
    {
        title.text = desc;
    }
}
