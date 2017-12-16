using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGameButton : MonoBehaviour {

    public void Action()
    {
        SceneManager.LoadScene("game");
    }
}
