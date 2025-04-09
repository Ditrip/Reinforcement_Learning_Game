using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelScr : MonoBehaviour
{
    
    public GameObject platformPrefab;
    public GameObject fallingObjPrefab;
    public GameObject fadePlatformPrefab;
    public GameObject jumpWallPrefab;
    public GameObject pillarsPrefab;
    public GameObject stepsBonusPrefab;
    public GameObject target;
    public GameObject agent;
    public GameObject rootPlatform;
    public int numOfRepeatedLvl = 5;
    public bool walls = false;
    public int spawnChanceUniquePlat = 30;
    public int stepsBonusSpawnInterval = 2;
    private List<GameObject> _platformList;
    private bool _checkPillarPlatformSpawn; // Is used to prevent spawn jump wall platform after platform with pillars
    // because the pillar can block the path for agent
    
    private static int _lvlCounter = 0;
    private uint _level;
    
    public readonly struct ObstNum //This struct is used to collect statistics
    {
        public ObstNum(int defaultNum, int pillarNum, int jumpWallNum)
        {
            DefaultNum = defaultNum;
            PillarNum = pillarNum;
            JumpWallNum = jumpWallNum;
        }
        public int DefaultNum { get; }
        public int PillarNum { get; }
        public int JumpWallNum { get; }
    }

    public void Start()
    {
        SetLevel();
        _lvlCounter = 0;
        MyPlayerPrefs.GetInstance().level = 1;
        MyPlayerPrefs.Save();
    }
    public void SetLevel()
    {
        _platformList ??= new List<GameObject>(); // ??= (Is Platform list null?)
        _checkPillarPlatformSpawn = false;
        _level = MyPlayerPrefs.GetInstance().GetLevel();
        Const.Direction prevDir = Const.Direction.Up;
        GameObject parentPlatform = rootPlatform;
        parentPlatform.GetComponent<PlatformWalls>().SetWallsActive(walls);
        parentPlatform.GetComponent<PlatformWalls>().SetHeight(Const.WallHeight.High);
        ResetLevel();
        // Train Level script is used for spawn certain obstacle platform
        TrainLevelScr trainLevelScr = gameObject.GetComponent<TrainLevelScr>();
        if (trainLevelScr is not null)
            trainLevelScr.isOdd = true;

        for (int i = 0; i < _level; i++)
        {
            GameObject platform;
            if (trainLevelScr is null)
            {
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
                    platformPos.z += Const.PlatformSize;
                    break;
                case Const.Direction.Left:
                    platformPos.x -= Const.PlatformSize;
                    break;
                case Const.Direction.Right:
                    platformPos.x += Const.PlatformSize;
                    break;
            }
            IsJumpPlatform(Const.GetOppositeDirection(newDir));

            platform.transform.position = platformPos;
            if(!walls)
                CheckMergePlatform(platformPos);
            
            if(i%stepsBonusSpawnInterval == 0 && stepsBonusPrefab)
                SetStepsBonus(platform);
            
            parentPlatform = platform;
            prevDir = newDir;
        }
        
        SetTarget(parentPlatform);
        SetAgent();
    }

    private void CheckMergePlatform(Vector3 childPlatformPos)
    {
        if (_platformList.Count > 2)
        {
            if (Math.Abs(childPlatformPos.x - rootPlatform.transform.position.x) < 0.1f &&
                childPlatformPos.z - rootPlatform.transform.position.z <= 10)
            {
                _platformList[^1].GetComponent<PlatformWalls>().WallHandle(Const.Direction.Down,true);
                _platformList[^1].GetComponent<PlatformWalls>().SetWallHeight(Const.Direction.Down,Const.WallHeight.High);
                return;
            }
            for (int i = _platformList.Count - 3; i >= 0; i--)
            {
                if (Math.Abs(childPlatformPos.x - _platformList[i].transform.position.x) < 0.1f &&
                    childPlatformPos.z - _platformList[i].transform.position.z <= 10)
                {
                    _platformList[^1].GetComponent<PlatformWalls>().WallHandle(Const.Direction.Down,true);
                    _platformList[^1].GetComponent<PlatformWalls>().SetWallHeight(Const.Direction.Down,Const.WallHeight.High);
                    break;
                }
            }
        }
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
        Vector3 targetPos = SetRandomPos(platform);
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

    private void SetStepsBonus(GameObject platform)
    {
        Vector3 pos = SetRandomPos(platform);
        pos.y += Const.TargetSize / 2;
        GameObject stepBonus = Instantiate(stepsBonusPrefab, platform.transform);
        stepBonus.transform.position = pos;
    }

    private Vector3 SetRandomPos(GameObject platform)
    {
        Vector3 pos  = platform.transform.position;
        pos.x += Random.Range(-(Const.PlatformSize / 2) + Const.TargetSize / 2,
            (Const.PlatformSize / 2) - Const.TargetSize / 2);
        pos.z += Random.Range(-(Const.PlatformSize / 2) + Const.TargetSize / 2,
            (Const.PlatformSize / 2) - Const.TargetSize / 2);
        return pos;
    }

    public void SetNextLevel()
    {
        if (_lvlCounter >= MyPlayerPrefs.GetInstance().level && _lvlCounter >= numOfRepeatedLvl)
        {
            MyPlayerPrefs.GetInstance().SetNextLevel();
            _lvlCounter = 0;
        }
        else{
            if(_level == MyPlayerPrefs.GetInstance().GetLevel())
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
        GameObject platformObj = null;
        // Debug.Log("Level scr(Random Value: " + chanceSpawn + ")");

        if (chanceSpawn <= spawnChanceUniquePlat)
        {
            Array platforms = Enum.GetValues(typeof(Const.Platforms));
            // Range set from 1 because 0 equals to default platform
            Const.Platforms platform = (Const.Platforms)platforms.GetValue(Random.Range(1, platforms.Length));
            // Debug.Log("Unique platform has spawned (Platform: " + platform.ToString() + ")");
            if (_checkPillarPlatformSpawn && platform == Const.Platforms.JumpWall)
            {
                platform = Const.Platforms.Default;
                _checkPillarPlatformSpawn = false;
            }


            switch (platform)
            {
                case Const.Platforms.JumpWall:
                    platformObj = Instantiate(jumpWallPrefab, gameObject.transform);
                    break;
                case Const.Platforms.Pillars:
                    platformObj = Instantiate(pillarsPrefab, gameObject.transform);
                    _checkPillarPlatformSpawn = true;
                    break;
                default:
                    Debug.Log("Level Scr (Unique platform set to 'default')");
                    platformObj = Instantiate(platformPrefab, gameObject.transform);
                    break;
            }
            
        }

        if (platformObj is null)
        {
            _checkPillarPlatformSpawn = false;
            platformObj = Instantiate(platformPrefab, gameObject.transform);
        }
        
        
        return platformObj;
    }

    public int GetPlatformIDFromList(GameObject platform)
    {
        int id = _platformList.IndexOf(platform);
        SetPlatformTagToPrev(id);
        return id;
    }

    private void SetPlatformTagToPrev(int lastID)
    {
        for (int i = 0; i < lastID; i++)
        {
            _platformList[i].tag = Const.Tags.PrevPlatform.ToString();
        }
    }

    public ObstNum GetNumberOfObstacles()
    {
        int defaultPlat = 0;
        int pillarPlat = 0;
        int jumpWallPlat = 0;
        foreach (GameObject platform in _platformList)
        {
            switch (platform.GetComponent<Platform>().GetPlatform())
            {
                case Const.Platforms.Default:
                    defaultPlat++;
                    break;
                case Const.Platforms.Pillars:
                    pillarPlat++;
                    break;
                case Const.Platforms.JumpWall:
                    jumpWallPlat++;
                    break;
            }
        }

        return new ObstNum(defaultPlat,pillarPlat,jumpWallPlat);
    }
    
    public uint GetLevel()
    {
        return _level;
    }
}
