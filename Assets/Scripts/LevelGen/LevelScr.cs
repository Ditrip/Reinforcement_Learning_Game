using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelScr : MonoBehaviour
{
    
    public GameObject platformPrefab;
    public GameObject fallingObjPrefab;
    public GameObject fadePlatformPrefab;
    public GameObject jumpWallPrefab;
    public GameObject pillarsPrefab;
    public GameObject target;
    public GameObject agent;
    public GameObject rootPlatform;
    public int numOfRepeatedLvl = 5;
    public bool walls = false;
    public int spawnChanceUniquePlat = 30;
    private List<GameObject> _platformList;
    private bool _checkUniquePlatformSpawn; // Is used to prevent spawn of two unique platform in row
    
    private static int _lvlCounter = 0;

    public void Start()
    {
        SetLevel();
        _lvlCounter = 0;
        MyPlayerPrefs.GetInstance().level = 1;
        MyPlayerPrefs.Save();
        // GameObject rootPlatform = GameObject.Find("RootPlatform");
        // _platformList.Add(rootPlatform);
    }
    public void SetLevel()
    {
        _platformList ??= new List<GameObject>(); // ??= (Is Platform list null?)
        _checkUniquePlatformSpawn = false;
        MyPlayerPrefs.PlayerStats playerStats = MyPlayerPrefs.GetInstance();
        uint level = playerStats.GetLevel();
        Const.Direction prevDir = Const.Direction.Up;
        GameObject parentPlatform = rootPlatform;
        parentPlatform.GetComponent<PlatformWalls>().SetWallsActive(walls);
        parentPlatform.GetComponent<PlatformWalls>().SetHeight(Const.WallHeight.High);
        ResetLevel();
        // Train Level script is used for spawn certain obstacle platform
        TrainLevelScr trainLevelScr = gameObject.GetComponent<TrainLevelScr>();
        if (trainLevelScr is not null)
            trainLevelScr.isOdd = true;

        for (int i = 0; i < level; i++)
        {
            GameObject platform;
            if (trainLevelScr is null)
            {
                // platform = Instantiate(platformPrefab, gameObject.transform);
                platform = GetRandomPlatform();
            }
            else
            {
                platform = trainLevelScr.InstantiatePlatform();
            }
            platform.transform.position = new Vector3(0, 0, 0);
            _platformList.Add(platform);
            
            PlatformWalls parentPlatformWalls = parentPlatform.GetComponent<PlatformWalls>();
            PlatformWalls childPlatformWalls = platform.GetComponent<PlatformWalls>();
            // By default all walls are active
            childPlatformWalls.SetWallsActive(walls);
            childPlatformWalls.SetHeight(Const.WallHeight.High);
            
            // Generates random direction of next platform (Up,Left,Right (See Const enum))
            Const.Direction newDir = (Const.Direction) Random.Range(0, 3);

            Vector3 platformPos = parentPlatform.transform.position;

            // This if statement used for preventing opposite directions (except up dir)
            // e.g if prev platform has spawned on the right
            // it can not spawn on the left (same position of prev platform)
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
            
            parentPlatformWalls.WallHandle(newDir,false);
            // Declare anonymous method
            void IsJumpPlatform(Const.Direction dir)
            {
                if (childPlatformWalls.TryGetComponent<JumpWall>(out JumpWall jumpWall)){
                    jumpWall.SetJumpWall(dir);
                    childPlatformWalls.SetWallHeight(dir,Const.WallHeight.Normal);
                }
                else
                    childPlatformWalls.WallHandle(dir, false);
            }

            switch (newDir)
            {
                case Const.Direction.Up:
                    IsJumpPlatform(Const.Direction.Down);
                    platformPos.z += Const.PlatformSize;
                    break;
                case Const.Direction.Left:
                    IsJumpPlatform(Const.Direction.Right);
                    platformPos.x -= Const.PlatformSize;
                    break;
                case Const.Direction.Right:
                    IsJumpPlatform(Const.Direction.Left);
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
        
    }
    private void SetTarget(GameObject platform)
    {
        Vector3 targetPos = platform.transform.position;
        targetPos.x += Random.Range(-(Const.PlatformSize / 2) + Const.TargetSize / 2,
            (Const.PlatformSize / 2) - Const.TargetSize / 2);
        targetPos.z += Random.Range(-(Const.PlatformSize / 2) + Const.TargetSize / 2,
            (Const.PlatformSize / 2) - Const.TargetSize / 2);
        targetPos.y += Const.TargetSize / 2;
        target.transform.position = targetPos;
    }
    
    private void SetAgent()
    {
        Vector3 pos = (agent.transform.position = rootPlatform.transform.position);
        pos.y += 0.5f;
        Rigidbody sphereBody = agent.GetComponent<Rigidbody>();
        sphereBody.velocity = Vector3.zero;
        sphereBody.angularVelocity = Vector3.zero;
    }

    public void SetNextLevel()
    {
        if (_lvlCounter >= MyPlayerPrefs.GetInstance().level && _lvlCounter >= numOfRepeatedLvl)
        {
            MyPlayerPrefs.GetInstance().SetNextLevel();
            _lvlCounter = 0;
        }
        else{
            _lvlCounter++;
            Debug.Log("Repeated level: "+ _lvlCounter + " " + 
                      (MyPlayerPrefs.GetInstance().level > numOfRepeatedLvl ? 
                          MyPlayerPrefs.GetInstance().level : numOfRepeatedLvl));
        }
        
        SetLevel();
    }

    private GameObject GetRandomPlatform()
    {
        int chanceSpawn = Random.Range(0, 101);
        // Debug.Log("Level scr(Random Value: " + chanceSpawn + ")");

        if (chanceSpawn <= spawnChanceUniquePlat && !_checkUniquePlatformSpawn)
        {
            Array platforms = Enum.GetValues(typeof(Const.Platforms));
            // Range set from 1 because 0 equals to default platform
            Const.Platforms platform = (Const.Platforms)platforms.GetValue(Random.Range(1, platforms.Length));
            // Debug.Log("Unique platform has spawned (Platform: " + platform.ToString() + ")");

            GameObject platformObj;

            switch (platform)
            {
                // case Const.Platforms.FallingObj:
                //     platformObj = Instantiate(fallingObjPrefab, gameObject.transform);
                //     break;
                // case Const.Platforms.FadePlatform:
                //     platformObj = Instantiate(fadePlatformPrefab, gameObject.transform);
                //     break;
                case Const.Platforms.JumpWall:
                    platformObj = Instantiate(jumpWallPrefab, gameObject.transform);
                    break;
                case Const.Platforms.Pillars:
                    platformObj = Instantiate(pillarsPrefab, gameObject.transform);
                    break;
                default:
                    Debug.Log("Level Scr (Unique platform set to 'default')");
                    platformObj = Instantiate(platformPrefab, gameObject.transform);
                    break;
            }

            _checkUniquePlatformSpawn = true;
            return platformObj;
        }

        _checkUniquePlatformSpawn = false;
        return Instantiate(platformPrefab, gameObject.transform);
    }

}
