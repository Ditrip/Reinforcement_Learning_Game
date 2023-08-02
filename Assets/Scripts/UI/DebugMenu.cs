using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public void NextLevelButton()
    {
        MyPlayerPrefs.PlayerStats instance = MyPlayerPrefs.GetInstance();
        instance.SetNextLevel();
        SetLevelAiEnv();
    }

    public void PrevLevelButton()
    {
        MyPlayerPrefs.PlayerStats instance = MyPlayerPrefs.GetInstance();
        if (instance.GetLevel() > 0)
        {
            instance.SetPrevLevel();
            SetLevelAiEnv();
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
        foreach (GameObject trainAiEnv in GameObject.FindGameObjectsWithTag(Const.Tags.Agent.ToString()))
        {
            MyAgent levelGenerator = trainAiEnv.GetComponent<MyAgent>();
            levelGenerator.EndEpisode();
        }
    }

    public void SetGameSpeed(GameObject sliderObj)
    {
        Slider slider = sliderObj.GetComponent<Slider>();
        Time.timeScale = slider.value;
        slider.GetComponent<GSSlider>().ValueChanged();
        
    }
}
