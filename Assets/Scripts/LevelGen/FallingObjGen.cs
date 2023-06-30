using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FallingObjGen : MonoBehaviour
{
    public GameObject fallingCube;

    public GameObject fallingSphere;

    public float frequency = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DropObject());
    }

    IEnumerator DropObject()
    {
        while (true)
        {
            int id = Random.Range(1, 3);
            Vector3 spawnPos = gameObject.transform.position;
            Vector3 randomPos = new Vector3(Random.Range(-5.0f, 5.0f), 13, Random.Range(-5.0f, 5.0f));
            Debug.Log("Falling Obj (id): " + id);
            if (id == 1)
            {
                
                Instantiate(fallingCube.gameObject,
                    spawnPos + randomPos,
                    Quaternion.identity, 
                    gameObject.transform);
            }
            else
            {
                Instantiate(fallingSphere.gameObject,
                    spawnPos + randomPos,
                    Quaternion.identity, 
                    gameObject.transform);
            }
            yield return new WaitForSeconds(frequency);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
