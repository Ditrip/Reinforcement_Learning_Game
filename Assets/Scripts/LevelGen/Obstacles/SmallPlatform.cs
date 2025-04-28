using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmallPlatform : MonoBehaviour
{

    public GameObject smallPlatform;
    
    public void SetSmallPlatform()
    {
        
        Vector3 scale = gameObject.transform.localScale;
        Vector3 pos = gameObject.transform.position;
        
        scale.x = Const.PlatformSize / 25;
        scale.z = Const.PlatformSize / 25;
        
        pos.z += Random.Range(-Const.PlatformSize/10, Const.PlatformSize/10);
        pos.x += Random.Range(-Const.PlatformSize/10, Const.PlatformSize/10);
        smallPlatform.transform.localScale = scale;
        smallPlatform.transform.position = pos;
        
    }

    void Start()
    {
        SetSmallPlatform();
    }
}
