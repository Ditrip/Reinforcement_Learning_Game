using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScr : MonoBehaviour
{
    private bool _pass = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<MyAgent>(out MyAgent myAgent))
        {
            if (!_pass)
            {
                myAgent.killAgent = true;
            }
        }
    }

    public void SetPathPass(bool pass)
    {
        _pass = pass;
    }
}
