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
}
