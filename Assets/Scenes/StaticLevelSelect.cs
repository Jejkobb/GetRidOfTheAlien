using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLevelSelect: MonoBehaviour
{
    public void LoadLevelWithName(string levelname)
    {
        FindObjectOfType<LevelManager>().LoadLevelByName(levelname);
    }

    public void ReloadLevel()
    {
        FindObjectOfType<LevelManager>().ReloadLevel();
    }
}
