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
    public float forceMultiplier = 1;
    public float jumpVelocity = 6;
    [HideInInspector]
    public LevelScr levelScr;
    [HideInInspector] 
    public bool killAgent;
    private bool _isAgentReachGoal;
    private bool _isAgentFell;
    private bool _isAgentTouchingWall;
    private bool _isAgentJumping;
    private bool _checkCoroutineJump;
    private bool _isAgentFailed;
    // private int _platformID;


    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.TryGetComponent<LevelScr>(out LevelScr levelScript))
        {
            levelScr = gameObject.GetComponentInParent<LevelScr>();
        }else if (transform.parent.TryGetComponent<TrainLevelScr>(out TrainLevelScr trainLevelScr))
        {
            levelScr = trainLevelScr;
        }
        
        _rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Vector3 agentPos = gameObject.transform.localPosition;
        // Vector3 agentRotation = gameObject.transform.rotation.eulerAngles;
        // Vector3 targetPos = levelScr.target.transform.localPosition;
        // sensor.AddObservation(agentPos.x);
        // sensor.AddObservation(agentPos.z);
        // sensor.AddObservation(targetPos.x);
        // sensor.AddObservation(targetPos.z);
        sensor.AddObservation(_rigidBody.velocity.x);
        sensor.AddObservation(_rigidBody.velocity.z);
        sensor.AddObservation(_isAgentTouchingWall);
        sensor.AddObservation(_isAgentFell);
        sensor.AddObservation(_isAgentReachGoal);
        sensor.AddObservation(_isAgentJumping);
        sensor.AddObservation(_isAgentFailed);
        // sensor.AddObservation(_platformID);
        //This observation is used to inform the agent how far he is from last platform
        // sensor.AddObservation(Vector3.Distance(targetPos,_lastPlatform.transform.localPosition));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0];
        int rotateAction = actions.DiscreteActions[1];
        int jumpAction = actions.DiscreteActions[2];

        MoveAgent(moveAction,rotateAction,jumpAction);

        if (_rigidBody.position.y < levelScr.rootPlatform.transform.position.y)
        {
            if (!_isAgentFell){
                _isAgentFell = true;
                return;
            }
            SetReward(-1);
            EndEpisode();
            return;
        }
        
        if (killAgent)
        {
            if (!_isAgentFailed)
            {
                _isAgentFailed = true;
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
            levelScr.SetNextLevel();
            EndEpisode();
            _isAgentReachGoal = false;
            return;
        }

        AddReward(-0.001f);
    }

    private void MoveAgent(int moveAct, int rotateAct, int jumpAction)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        switch (moveAct)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                _rigidBody.AddForce(dirToGo * forceMultiplier,
                    ForceMode.VelocityChange);
                break;
            case 2:
                _rigidBody.velocity = new Vector3(0,_rigidBody.velocity.y,0);
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

        switch (jumpAction)
        {
            case 1:
                // Debug.Log("Jump action received(_isAgentJumping: " + _isAgentJumping + ")");
                if(!_isAgentJumping){
                    _rigidBody.AddForce(0,jumpVelocity,0,ForceMode.VelocityChange);
                    _isAgentJumping = true;
                    StartCoroutine(JumpingCoroutine());
                }
                break;
        }
        
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
    }

    public override void OnEpisodeBegin()
    {
        levelScr.SetLevel();
        _isAgentTouchingWall = false;
        _isAgentFell = false;
        _isAgentJumping = false;
        _checkCoroutineJump = true;
        _isAgentFailed = false;
        killAgent = false;
        // _platformID = (int)levelScr.rootPlatform.GetComponent<Platform>().GetPlatform();
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
        } else if (Input.GetKey(KeyCode.Space))
        {
            discreteActions[2] = 1;
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Const.Tags.NextPlatform.ToString()))
        {
            
            if (collision.gameObject.TryGetComponent<Platform>(out Platform platform)){
                platform.RewardAgent(gameObject);
                // _platformID = (int)platform.GetPlatform();
                // Debug.Log("Current platform ID: " + _platformID);
            }
            else
                Debug.LogWarning("Tagged object ("+Const.Tags.NextPlatform + ") does not have required script (Platform)");

            return;
        }

        if (collision.gameObject.tag.Equals(Const.Tags.Wall.ToString()) ||
            collision.gameObject.tag.Equals(Const.Tags.JumpWall.ToString()))
        {
            _isAgentTouchingWall = true;
            AddReward(-0.05f);
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
            if (other.gameObject.tag.Equals(Const.Tags.Wall.ToString())||
                other.gameObject.tag.Equals(Const.Tags.JumpWall.ToString()))
            {
                _isAgentTouchingWall = false;
            }
        }
    }

    public void OnCollisionStay(Collision collisionInfo)
    {
        if (_checkCoroutineJump)
        {
            if (collisionInfo.gameObject.tag.Equals(Const.Tags.Platform.ToString())
                || collisionInfo.gameObject.tag.Equals(Const.Tags.RootPlatform.ToString()))
            {
                _isAgentJumping = false;
            }
        }
    }

    private IEnumerator JumpingCoroutine()
    {
        _checkCoroutineJump = false;
        yield return new WaitForSeconds(1);
        _checkCoroutineJump = true;
    }
}
