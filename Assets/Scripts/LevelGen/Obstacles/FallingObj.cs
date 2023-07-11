using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObj : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.y < 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Const.Tags.Agent.ToString()))
        {
            collision.gameObject.GetComponent<MyAgent>().levelScr.SetLevel();
        }
    }
}
