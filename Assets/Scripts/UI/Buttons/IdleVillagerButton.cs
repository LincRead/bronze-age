using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IdleVillagerButton : MonoBehaviour {

    Button _button;
    Image _image;

    int idleVillagerIndex = 0;

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

            _number.enabled = true;
            _number.text = PlayerManager.instance.idleVillagers.Count.ToString();

            if(Input.GetKeyDown(KeyCode.I))
            {
                ToggleIdleVillager();
            }
        }

        else
        {
            _image.enabled = false;
            _button.interactable = false;
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
        UnitStateController villager = PlayerManager.instance.idleVillagers[idleVillagerIndex];
        PlayerManager.instance._controllerSelecting.ResetSelection();
        Grid.instance.selectedTilePrefab.GetComponent<SpriteRenderer>().enabled = false;

        PlayerManager.instance.selectableController = villager;
        PlayerManager.instance._controllerSelecting.SetUnitAsSelected();

        CameraController.instance.MoveToController(villager);

        idleVillagerIndex++;
    }
}
