using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScr : MonoBehaviour
{
    public void NextLevelButton()
    {
        MyPlayerPrefs.PlayerStats instance = MyPlayerPrefs.GetInstance();
        instance.SetNextLevel();
        // SetLevelAiEnv();
    }

    public void PrevLevelButton()
    {
        MyPlayerPrefs.PlayerStats instance = MyPlayerPrefs.GetInstance();
        if (instance.GetLevel() > 0)
        {
            instance.SetPrevLevel();
            // SetLevelAiEnv();
        }
    }

    public void ResetButton()
    {
        MyPlayerPrefs.PlayerStats instance = MyPlayerPrefs.GetInstance();
        instance.Reset();
        // SetLevelAiEnv();
    }

    private void SetLevelAiEnv()
    {
        foreach (GameObject AiEnv in GameObject.FindGameObjectsWithTag("AiEnv"))
        {
            MainScr levelGenerator = AiEnv.GetComponent<MainScr>();
            levelGenerator.SetLevel();
        }
    }
}
