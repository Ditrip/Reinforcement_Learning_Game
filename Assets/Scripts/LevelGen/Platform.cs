using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    private Const.Platforms platform = Const.Platforms.Default;
    private List<GameObject> _agentList;
    // 'Const.Direction?' allow the data type to be nullable 
    private Const.Direction? _dir = null;
    private GameObject _parentPlatform = null;

    // 'Properties' are used for encapsulation 
    public Const.Direction Direction
    {
        get
        {
            if (_dir is null)
            {
                Debug.LogWarning("Platform (dir is not set (null)) returned Up Dir");
                return Const.Direction.Up;
            }
            return (Const.Direction)_dir;
        }
        set => _dir = value;

    }
    public GameObject ParentPlatform
    {
        get
        {
            if (_parentPlatform is null)
            {
                Debug.LogWarning(gameObject.tag.Equals(Const.Tags.RootPlatform.ToString())
                    ? "Platform (you trying to get parent platform from root platform"
                    : "Platform (parentPlatform is not set (null))");
                return null;
            }
            return _parentPlatform;
        }
        set => _parentPlatform = value;
    }
    
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
