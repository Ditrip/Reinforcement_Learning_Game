using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBonus : MonoBehaviour
{
    [Range(0, 200)] 
    public int bonusTime = 50;

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
