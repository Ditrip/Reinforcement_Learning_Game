using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualControl : MonoBehaviour
{
    public float forceMultiplier = 1;
    public MainScr levelControl;
    
    private Rigidbody _rigidbody;
    private Vector3 _dir;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _dir = Vector3.zero;
        levelControl = gameObject.GetComponentInParent<MainScr>();
    }

    // Update is called once per frame
    void Update()
    {
        float horAxis = Input.GetAxis("Horizontal");
        float verAxis = Input.GetAxis("Vertical");

        _dir.x = horAxis * forceMultiplier;
        _dir.z = verAxis * forceMultiplier;

        if (gameObject.transform.position.y < 0)
        {
            levelControl.SetLevel();
        }
        
        _rigidbody.AddForce(_dir);
        
    }

    public void ResetPosition(GameObject rootPlatform)
    {
        Vector3 origPos = rootPlatform.transform.position;
        origPos.y += 0.5f;
        gameObject.transform.position = origPos;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Const.Tags.Target.ToString()))
        {
            MyPlayerPrefs.GetInstance().SetNextLevel();
            MainScr.SetNextLevel();
        }
    }
}
