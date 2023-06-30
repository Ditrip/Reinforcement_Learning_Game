using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class MyAgent : Agent
{
    private Rigidbody _rigidBody;
    public float forceMultiplier;
    private LevelScr _levelScr;
    private bool _isAgentReachGoal;
    private bool _isAgentFell;
    private bool _isAgentTouchingWall;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _levelScr = gameObject.GetComponentInParent<LevelScr>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameObject.transform.localPosition);
        sensor.AddObservation(_levelScr.target.transform.localPosition);
        sensor.AddObservation(_rigidBody.velocity.x);
        sensor.AddObservation(_rigidBody.velocity.z);
        sensor.AddObservation(_levelScr.walls ? _isAgentTouchingWall : _isAgentFell);
        sensor.AddObservation(_isAgentReachGoal);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0];
        int rotateAction = actions.DiscreteActions[1];
        
        MoveAgent(moveAction,rotateAction);

        if (_rigidBody.position.y < _levelScr.rootPlatform.transform.position.y)
        {
            if (!_isAgentFell){
                _isAgentFell = true;
                return;
            }
            SetReward(-1);
            EndEpisode();
            return;
        }

        if (_isAgentReachGoal)
        {
            Debug.Log("Agent reach goal (OnActionReceived)");
            SetReward(10);
            _levelScr.SetNextLevel();
            EndEpisode();
            _isAgentReachGoal = false;
            return;
        }
        
        AddReward(-0.00001f);
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
            // case 2:
            //     dirToGo = transform.forward * -1f;
            //     break;
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
        _levelScr.SetLevel();
        _isAgentFell = false;
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
        if (collision.gameObject.tag.Equals(Const.Tags.NextPlatform.ToString()))
        {
            try
            {
                if (collision.gameObject.GetComponent<Platform>().RewardAgent(gameObject))
                {
                    AddReward(0.1f);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.LogWarning("Platform script is missing");
                throw;
            }
            return;
        }

        if (collision.gameObject.tag.Equals(Const.Tags.Wall.ToString()))
        {
            _isAgentTouchingWall = true;
            return;
        }
        
        if (collision.gameObject.tag.Equals(Const.Tags.Target.ToString()))
        {
            _isAgentReachGoal = true;
        }
    }

    public void OnCollisionExit(Collision other)
    {
        if (_isAgentTouchingWall)
        {
            if (other.gameObject.tag.Equals(Const.Tags.Wall.ToString()))
            {
                _isAgentTouchingWall = false;
            }
        }
    }
}
