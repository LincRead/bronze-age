using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour {

    public GameObject[] screens;
    GameObject lastScreen;
    GameObject currentScreen;
    int index = 0;

    private void Start()
    {
        for(int i = 0; i < screens.Length; i++)
        {
            screens[i].gameObject.SetActive(false);
        }

        lastScreen = screens[0];
        currentScreen = screens[0];
        currentScreen.gameObject.SetActive(true);
    }

    public void Action()
    {
        index++;

        if(index >= screens.Length)
        {
            StartGame();
            return;
        }

        lastScreen.gameObject.SetActive(false);
        currentScreen = screens[index];

        currentScreen.gameObject.SetActive(true);
        lastScreen = currentScreen;
    }

    void StartGame()
    {
        SceneManager.LoadScene("game");
    }
}
