using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsBonus : MonoBehaviour
{
    [Range(0, 200)] 
    public int bonusTime = 50;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Const.Tags.Agent.ToString()))
        {
            collision.gameObject.GetComponent<MyAgent>().IncreaseMaxSteps(bonusTime);
            Destroy(gameObject);
        }
    }
}
