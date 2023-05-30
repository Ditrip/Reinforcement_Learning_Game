using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MyAgent : Agent
{
    private Rigidbody _rigidBody;
    public float forceMultiplier;
    private MainScr _mainScr;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainScr = gameObject.GetComponentInParent<MainScr>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameObject.transform.localPosition);
        sensor.AddObservation(_mainScr.target.transform.localPosition);
        sensor.AddObservation(_rigidBody.velocity.x);
        sensor.AddObservation(_rigidBody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0];
        int rotateAction = actions.DiscreteActions[1];
        
        MoveAgent(moveAction,rotateAction);

        if (_rigidBody.position.y < _mainScr.rootPlatform.transform.position.y)
        {
            SetReward(-0.1f);
            EndEpisode();
        }
        
        SetReward(-0.0005f);

        // float distanceToTarget =
        //     Vector3.Distance(gameObject.transform.localPosition, _mainScr.target.transform.localPosition);
        // if (distanceToTarget < 1.4)
        // {
        //     SetReward(1);
        //     _mainScr.SetNextLevel();
        //     EndEpisode();
        // }
    }

    private void MoveAgent(int moveAct, int rotateAct)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        switch (moveAct)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
        }

        switch (rotateAct)
        {
            case 1:
                rotateDir = transform.up * 1f;
                break;
            case 2:
                rotateDir = transform.up * -1f;
                break;
        }
        
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        _rigidBody.AddForce(dirToGo * forceMultiplier,
            ForceMode.VelocityChange);
    }

    public override void OnEpisodeBegin()
    {
        _mainScr.SetLevel();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[0] = 1;
        } else if (Input.GetKey(KeyCode.S))
        {
            discreteActions[0] = 2;
        } else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[1] = 1;
        } else if (Input.GetKey(KeyCode.A))
        {
            discreteActions[1] = 2;
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Const.Tags.Target.ToString()))
        {
            SetReward(1);
            _mainScr.SetNextLevel();
            EndEpisode();
        }
    }
}
