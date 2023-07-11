using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainLevelScr : LevelScr
{
    public Const.Platforms platforms;

    // isOdd is used to make space between next obstacle
    [HideInInspector] 
    public bool isOdd;

    public GameObject InstantiatePlatform()
    {
        
        GameObject platform;
        
        if(!isOdd){
            platform = Instantiate(platformPrefab, gameObject.transform);
            isOdd = !isOdd;
            return platform;
        } 
        else 
            isOdd = !isOdd;
        
        switch (platforms)
        {
            case Const.Platforms.FallingObj:
                platform = Instantiate(fallingObjPrefab, gameObject.transform);
                break;
            case Const.Platforms.FadePlatform:
                platform = Instantiate(fadePlatformPrefab, gameObject.transform);
                break;
            case Const.Platforms.JumpWall:
                platform = Instantiate(jumpWallPrefab, gameObject.transform);
                break;
            default:
                Debug.Log("TrainLevelScr(Platform set to 'default')");
                platform = Instantiate(platformPrefab, gameObject.transform);
                break;
        }

        return platform;
    }
}
