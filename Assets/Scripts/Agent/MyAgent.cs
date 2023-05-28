using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class MyAgent : Unity.MLAgents.Agent
{
    private Rigidbody _rigidbody;

    private MainScr _mainScr;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainScr = gameObject.GetComponentInParent<MainScr>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        if (_rigidbody.position.y < 0)
        {
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }
}
