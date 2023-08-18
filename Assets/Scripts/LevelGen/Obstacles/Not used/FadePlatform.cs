using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePlatform : MonoBehaviour
{
    private MeshCollider _meshCollider;
    private MeshRenderer _meshRenderer;

    public float timeToFadeOut = 4;
    public float timeToFadeIn = 2;
    
    // Start is called before the first frame update
    void Start()
    {
        _meshCollider = gameObject.GetComponent<MeshCollider>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
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
        int counter = 0;
        while (true)
        {
            counter++;
            // Fixed update has fixed update time (0.02)
            // Every 50 calls 1 second in game passes
            // To fade out or in platform I set 4 (4/fixedTime = 4/0.02 = 200 calls) seconds until it will fade out
            yield return new WaitForFixedUpdate();
            if (counter >= timeToFadeOut/Time.fixedDeltaTime)
                break;
        }
        _meshCollider.enabled = false;
        _meshRenderer.enabled = false;
        StartCoroutine(FadeInPlatform());
    }

    private IEnumerator FadeInPlatform()
    {
        int counter = 0;
        while (true)
        {
            counter++;
            yield return new WaitForFixedUpdate();
            if (counter >= timeToFadeIn/Time.fixedDeltaTime)
                break;
        }
        _meshCollider.enabled = true;
        _meshRenderer.enabled = true;
    }
}
