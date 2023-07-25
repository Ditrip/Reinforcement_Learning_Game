using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Const.Tags.Agent.ToString()))
        {
            if (collision.gameObject.TryGetComponent<MyAgent>(out MyAgent myAgent))
                myAgent.killAgent = true;
            else
                Debug.LogWarning("Tagged object (Agent) does not have required script (MyAgent)");
        }
    }
}
