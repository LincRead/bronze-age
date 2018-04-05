using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMap : MonoBehaviour {

    public Slider _slider;

    private bool loadingWorld = false;
    private AsyncOperation asyncLoadWorld;

    void Start() {

    }

    public void LoadDemoScene()
    {
        gameObject.SetActive(true);

        // Wait a little bit while game versions have so little to load.
        Invoke("StartLoading", 0.3f);
    }

    void StartLoading()
    {
        loadingWorld = true;
        asyncLoadWorld = SceneManager.LoadSceneAsync("game");
        asyncLoadWorld.allowSceneActivation = false;
    }

    void Update()
    {
        if (loadingWorld)
        {
            _slider.value = asyncLoadWorld.progress / 0.9f;

            if (asyncLoadWorld.progress >= 0.9f)
            {
                Invoke("OpenLoadedScene", 1.2f);
                return;
            }
        }
    }

    void OpenLoadedScene()
    {
        asyncLoadWorld.allowSceneActivation = true;
    }
}
