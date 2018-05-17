using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour {

    public enum DIFFICULTY
    {
        EASY,
        MEDIUM,
        HARD
    }

    public DIFFICULTY difficulty = DIFFICULTY.EASY;

    public static PersistentData instance;

    private void Start()
    {
        if(PersistentData.instance)
        {
            Destroy(gameObject);
        }

        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }

    public void ChangeDifficulty(int value)
    {
        difficulty = (DIFFICULTY)value;
    }
}
