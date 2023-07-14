using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObj : MonoBehaviour
{

    public float gravity = 1.5f;
    
    private Rigidbody _rigidbody;
    private void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.y < 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(Vector3.down*(_rigidbody.mass*gravity));
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Const.Tags.Agent.ToString()))
        {
            collision.gameObject.GetComponent<MyAgent>().EndEpisode();
        }
    }
}
