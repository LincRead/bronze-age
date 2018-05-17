using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDifficultyLevel : MonoBehaviour {

	public void Action()
    {
        PersistentData.instance.ChangeDifficulty(GetComponent<Dropdown>().value);
    }
}
