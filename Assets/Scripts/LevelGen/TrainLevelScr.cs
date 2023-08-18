using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainLevelScr : LevelScr
{
    public Const.Platforms platforms;

    // isOdd is used to make space between next obstacle
    [HideInInspector] 
    public bool isOdd;

    public GameObject InstantiatePlatform(Const.Direction dir)
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
            case Const.Platforms.JumpWall:
                platform = Instantiate(jumpWallPrefab, gameObject.transform);
                break;
            case Const.Platforms.Pillars:
                platform = Instantiate(pillarsPrefab, gameObject.transform);
                break;
            case Const.Platforms.Path:
                platform = Instantiate(pathPrefab, gameObject.transform);
                break;
            default:
                Debug.Log("TrainLevelScr(Platform set to 'default')");
                platform = Instantiate(platformPrefab, gameObject.transform);
                break;
        }

        return platform;
    }
}
