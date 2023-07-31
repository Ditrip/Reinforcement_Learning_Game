using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    private Const.Platforms platform = Const.Platforms.Default;
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
                // agent.GetComponent<MyAgent>().AddReward(-0.1f);
                return false;
            }
        }
        Debug.Log("Agent is rewarded");
        _agentList.Add(agent);
        // agent.GetComponent<MyAgent>().AddReward(0.5f);
        gameObject.tag = Const.Tags.Platform.ToString();
        return true;
    }

    public Const.Platforms GetPlatform()
    {
        return platform;
    }
}
