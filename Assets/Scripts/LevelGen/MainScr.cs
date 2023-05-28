using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainScr : MonoBehaviour
{
    
    public GameObject platformPrefab;
    public GameObject targetPrefab;
    public GameObject agentPrefab;
    public GameObject rootPlatform;
    private List<GameObject> _platformList;
    // private MyPlayerPrefs.PlayerStats _playerStats;
    private GameObject _target;

    public void Start()
    {
        SetLevel();
        // GameObject rootPlatform = GameObject.Find("RootPlatform");
        // _platformList.Add(rootPlatform);
    }
    public void SetLevel()
    {
        if (_platformList is null)
        {
            _platformList = new List<GameObject>();
        }
        MyPlayerPrefs.PlayerStats _playerStats = MyPlayerPrefs.GetInstance();
        uint level = _playerStats.GetLevel();
        Const.Direction prevDir = Const.Direction.Up;
        GameObject parentPlatform = rootPlatform;
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
        SetAgent();
    }
    private void ResetLevel()
    {
        
        foreach (GameObject platform in _platformList)
        {
            Destroy(platform);
        }
        _platformList.Clear();
        
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
    
    private void SetAgent()
    {
        ManualControl _control = gameObject.GetComponentInChildren<ManualControl>();
        Vector3 pos = (agentPrefab.transform.position = rootPlatform.transform.position);
        pos.y += 0.5f;
        Rigidbody sphereBody = agentPrefab.GetComponent<Rigidbody>();
        sphereBody.velocity = Vector3.zero;
        sphereBody.angularVelocity = Vector3.zero;
    }

    public static void SetNextLevel()
    {
        foreach (GameObject aiEnv in GameObject.FindGameObjectsWithTag(Const.Tags.AiEnv.ToString()))
        {
            MainScr levelGen = aiEnv.GetComponent<MainScr>();
            levelGen.SetLevel();
        }
    }
}
