using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private List<GameObject> _agentList;
    private void Awake()
    {
        _agentList = new List<GameObject>();
    }

    public bool RewardAgent(GameObject agent)
    {
        foreach(GameObject agentList in _agentList)
        {
            if (agent == agentList)
            {
                Debug.Log("Agent is already rewarded");
                return false;
            }
        }
        Debug.Log("Agent is rewarded");
        _agentList.Add(agent);
        gameObject.tag = Const.Tags.Platform.ToString();
        return true;
    }
}
