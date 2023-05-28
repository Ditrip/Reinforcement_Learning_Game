using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScr : MonoBehaviour
{

    public float speedMultiplier = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = gameObject.transform.position;
        
        if (Input.GetKey(KeyCode.I))
        {
            cameraPos.z += speedMultiplier * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.K))
        {
            cameraPos.z -= speedMultiplier * Time.deltaTime;
        }  else if (Input.GetKey(KeyCode.J))
        {
            cameraPos.x += speedMultiplier * Time.deltaTime;
        }  else if (Input.GetKey(KeyCode.L))
        {
            cameraPos.x -= speedMultiplier * Time.deltaTime;
        }

        gameObject.transform.position = cameraPos;
    }
}
