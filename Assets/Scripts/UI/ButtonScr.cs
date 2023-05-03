using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScr : MonoBehaviour
{
    public void NextLevelButton()
    {
        MyPlayerPrefs.PlayerStats instance = MyPlayerPrefs.GetInstance();
        
        instance.SetNextLevel();
        
    }

    public void PrevLevelButton()
    {
        MyPlayerPrefs.PlayerStats instance = MyPlayerPrefs.GetInstance();
        if (instance.GetLevel() > 0)
        {
            instance.SetPrevLevel();
        }
    }

    public void ResetButton()
    {
        MyPlayerPrefs.PlayerStats instance = MyPlayerPrefs.GetInstance();
        instance.Reset();
    }
}
