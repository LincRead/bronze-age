using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IdleVillagerButton : MonoBehaviour {

    Button _button;
    Image _image;

    int idleVillagerIndex = 0;

    public Image _subIcon;
    public Text _number;

    void Start()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
    }

    void Update ()
    {
		if(PlayerManager.instance.idleVillagers.Count > 0)
        {
            _image.enabled = true;
            _button.interactable = true;
            _subIcon.enabled = true;
            _number.enabled = true;

            _number.text = PlayerManager.instance.idleVillagers.Count.ToString();
        }

        else
        {
            _image.enabled = false;
            _button.interactable = false;
            _subIcon.enabled = false;
            _number.enabled = false;
        }
	}

    public void ToggleIdleVillager()
    {
        if(idleVillagerIndex >= PlayerManager.instance.idleVillagers.Count)
        {
            idleVillagerIndex = 0;
        }

        // Select next idle villager
        PlayerManager.instance._controllerSelecting.ResetSelection();
        PlayerManager.instance._controllerSelecting.SetControllerAsSelected(PlayerManager.instance.idleVillagers[idleVillagerIndex]);
        CameraController.instance.MoveToController(PlayerManager.instance.idleVillagers[idleVillagerIndex]);

        idleVillagerIndex++;
    }
}
