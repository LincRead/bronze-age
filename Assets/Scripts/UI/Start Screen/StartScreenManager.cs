using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour {

    public GameObject mainMenuElements;

    [Header("How to play")]
    public GameObject[] howToPlayPages;
    public GameObject previousButton;
    public GameObject nextButton;

    GameObject lastScreen;
    GameObject currentScreen;
    int currentHowToPlayPage = 0;

    private void Start()
    {
        for(int i = 0; i < howToPlayPages.Length; i++)
        {
            howToPlayPages[i].gameObject.SetActive(false);
        }

        lastScreen = howToPlayPages[0];
        currentScreen = howToPlayPages[0];

        ShowMainButtons();
    }

    public void OpenHowToPlayPages()
    {
        mainMenuElements.SetActive(false);

        // Reset how to play
        nextButton.SetActive(true);
        previousButton.SetActive(true);

        ShowCurrentHowToPlayPage();
    }

    void ShowCurrentHowToPlayPage()
    {
        currentScreen = howToPlayPages[currentHowToPlayPage];
        currentScreen.gameObject.SetActive(true);
        lastScreen = currentScreen;
    }

    public void OpenNextHowToPlayPage()
    {
        currentHowToPlayPage++;

        lastScreen.gameObject.SetActive(false);

        if (currentHowToPlayPage >= howToPlayPages.Length)
        {
            ShowMainButtons();
            return;
        }

        ShowCurrentHowToPlayPage();
    }

    public void OpenPreviousHowToPlayPage()
    {
        currentHowToPlayPage--;

        lastScreen.gameObject.SetActive(false);

        if (currentHowToPlayPage < 0)
        {
            ShowMainButtons();
            return;
        }

        ShowCurrentHowToPlayPage();
    }

    void ShowMainButtons()
    {
        mainMenuElements.SetActive(true);

        // Reset how to play
        nextButton.SetActive(false);
        previousButton.SetActive(false);
        currentHowToPlayPage = 0;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
