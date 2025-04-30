using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private int _currentPlatformID;
    private int _lastPlatformID;
    private uint _stepsCount;
    


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
    
    public override void OnEpisodeBegin()
    {
        levelScr.SetLevel();
        _isAgentTouchingWall = false;
        _isAgentFell = false;
        _isAgentJumping = false;
        _checkCoroutineJump = true;
        _isAgentFailed = false;
        killAgent = false;
        _currentPlatformID = -1;
        _lastPlatformID = -1;
        // MaxStep = MaxStepsOnStart;
        _stepsCount = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_rigidBody.velocity.x);
        sensor.AddObservation(_rigidBody.velocity.z);
        sensor.AddObservation(_isAgentTouchingWall);
        sensor.AddObservation(_isAgentFell);
        sensor.AddObservation(_isAgentReachGoal);
        sensor.AddObservation(_isAgentJumping);
        sensor.AddObservation(_isAgentFailed);
        sensor.AddObservation(_lastPlatformID);
        sensor.AddObservation(_currentPlatformID);
        //This observation is used to inform the agent how far he is from last platform
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
            // Debug.Log("Agent reach goal (OnActionReceived)");
            SetReward(10);
            if(levelScr.GetLevel() == MyPlayerPrefs.GetInstance().GetLevel())
                Statistic.GetInstance().CollectStats((int)levelScr.GetLevel(),_stepsCount,levelScr.GetNumberOfObstacles());
            levelScr.SetNextLevel();
            EndEpisode();
            _isAgentReachGoal = false;
            
            return;
        }

        AddReward(-0.001f);
        _stepsCount++;
        // if(_stepsCount%100 == 0)
        //     Debug.Log("Steps count: " + _stepsCount);
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
                    // StartCoroutine(JumpOverTime());
                    StartCoroutine(JumpingCoroutine());
                }
                break;
        }
        
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
    }
    
    private IEnumerator JumpOverTime()
    {
        float timer = 0f;
        float jumpDuration = 0.5f;
        while (timer < jumpDuration)
        {
            _rigidBody.AddForce(Vector3.up * (20f * Time.fixedDeltaTime), ForceMode.VelocityChange);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

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
        if (collision.gameObject.tag.Equals(Const.Tags.NextPlatform.ToString())
            || collision.gameObject.tag.Equals(Const.Tags.Platform.ToString())
            || collision.gameObject.tag.Equals(Const.Tags.PrevPlatform.ToString())
            || collision.gameObject.tag.Equals(Const.Tags.RootPlatform.ToString()))
        {
            SetPlatformID(collision.gameObject);
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
                || collisionInfo.gameObject.tag.Equals(Const.Tags.RootPlatform.ToString())
                || collisionInfo.gameObject.tag.Equals(Const.Tags.PrevPlatform.ToString())
                || collisionInfo.gameObject.tag.Equals(Const.Tags.NextPlatform.ToString())
                || collisionInfo.gameObject.tag.Equals(Const.Tags.OverLeapBlock.ToString()))
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

    private void SetPlatformID(GameObject platform)
    {
        int id = levelScr.GetPlatformIDFromList(platform);
        _lastPlatformID = id > _lastPlatformID ? id : _lastPlatformID;
        
        if (platform.gameObject.TryGetComponent<Platform>(out Platform platformScr)){
            if (platformScr.RewardAgent(gameObject))
            {
                AddReward(0.5f);
            }
            else
            {
                if (_lastPlatformID != id && _currentPlatformID != id && _currentPlatformID > id)
                {
                    //plus 2 is used to prevent wrong platform id difference calculation
                    //since root platform is -1
                    //e.g last = 1 , current = -1 : (1-(-1)) = 1 (wrong, should be 2, even with absolute value is wrong)
                    //e.g (with +2) last = 1 , current = -1 : ((2+1)-(2-1)) = 2
                    AddReward(-0.01f * ( (2 + _lastPlatformID) - (2 + id) ) );
                    // Debug.Log("Dif: " +((2 + _lastPlatformID) - (2 + id)) 
                    //     + " Reward: " + (-0.01f * ( (2 + _lastPlatformID) - (2 + id) ) ));
                }
            }
        }
        else
            Debug.LogWarning("Tagged object ("+platform.tag + ") does not have required script (Platform)");
        
        _currentPlatformID = id;

        // Debug.Log("Current platform ID: " + _currentPlatformID + " Last platform ID: " + _lastPlatformID);
    }
}
