using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePlatform : MonoBehaviour
{
    private MeshCollider _meshCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        _meshCollider = gameObject.GetComponent<MeshCollider>();
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Const.Tags.Agent.ToString()))
        {
            StartCoroutine(FadeOutPlatform());
        }
    }

    private IEnumerator FadeOutPlatform()
    {
        yield return new WaitForSeconds(2);
        _meshCollider.enabled = false;
        StartCoroutine(FadeInPlatform());
    }

    private IEnumerator FadeInPlatform()
    {
        yield return new WaitForSeconds(2);
        _meshCollider.enabled = true;
    }
}
