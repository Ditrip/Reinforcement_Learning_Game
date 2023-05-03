using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainScr : MonoBehaviour
{
    
    public GameObject platformPrefab;
    public GameObject targetPrefab;
    private List<GameObject> _platformList;
    private MyPlayerPrefs.PlayerStats _playerStats;
    private GameObject _target;

    public void Start()
    {
        _playerStats = MyPlayerPrefs.GetInstance();
        _platformList = new List<GameObject>();
        SetLevel();
        // GameObject rootPlatform = GameObject.Find("RootPlatform");
        // _platformList.Add(rootPlatform);
        
    }

    public void SetLevel()
    {
        
        uint level = _playerStats.GetLevel();
        Const.Direction prevDir = Const.Direction.Up;
        GameObject parentPlatform = GameObject.Find("RootPlatform");
        ResetLevel();

        for (int i = 0; i < level; i++)
        {
            GameObject platform = Instantiate(platformPrefab,gameObject.transform);
            platform.transform.position = new Vector3(0, 0, 0);
            _platformList.Add(platform);
            Const.Direction newDir = (Const.Direction) Random.Range(0, 3);

            Vector3 platformPos = parentPlatform.transform.position;

            if (prevDir != newDir && newDir != Const.Direction.Up)
            {
                if (newDir == Const.Direction.Left)
                {
                    newDir = (Const.Direction)Random.Range(1, 3);
                }
                else if (newDir == Const.Direction.Right)
                {
                    newDir = (Const.Direction)Random.Range(0, 2);
                }
                
            }
            
            Debug.Log("Directions: " + newDir);
            
            switch (newDir)
            {
                case Const.Direction.Up:
                    platformPos.z += Const.PlatformSize;
                    break;
                case Const.Direction.Left:
                    platformPos.x -= Const.PlatformSize;
                    break;
                case Const.Direction.Right:
                    platformPos.x += Const.PlatformSize;
                    break;
            }

            platform.transform.position = platformPos;

            parentPlatform = platform;
            prevDir = newDir;
        }
        
        SetTarget(parentPlatform);
        
    }

    private void ResetLevel()
    {
        foreach (GameObject platform in _platformList)
        {
            GameObject.Destroy(platform);
        }
        
        if(_target is not null)
            Destroy(_target);
    }

    private void SetTarget(GameObject platform)
    {
        Vector3 targetPos = platform.transform.position;

        targetPos.x += Random.Range(-(Const.PlatformSize/2)+Const.TargetSize/2,(Const.PlatformSize/2)-Const.TargetSize/2 );
        targetPos.z += Random.Range(-(Const.PlatformSize/2)+Const.TargetSize/2,(Const.PlatformSize/2)-Const.TargetSize/2 );
        targetPos.y += Const.TargetSize / 2;

        _target = Instantiate(targetPrefab,gameObject.transform);
        _target.transform.position = targetPos;
    }
}
