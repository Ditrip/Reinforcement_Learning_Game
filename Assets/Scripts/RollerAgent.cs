using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Vector3 = UnityEngine.Vector3;

public class RollerAgent : Agent
{
    private Rigidbody rBody;
    public Transform Target;
    // Start is called before the first frame update
    void Start()
    {
        rBody = gameObject.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
            transform.localPosition = new Vector3( 0, 0.5f, 0);
        }
        
        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4,
            0.5f,
            Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 agentAction = Vector3.zero;
        agentAction.x = actions.ContinuousActions[0];
        agentAction.z = actions.ContinuousActions[1];
        rBody.AddForce( agentAction * forceMultiplier);

        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);
        if (distanceToTarget < 1.42)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}
