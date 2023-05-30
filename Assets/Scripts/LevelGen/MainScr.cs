using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainScr : MonoBehaviour
{
    
    public GameObject platformPrefab;
    public GameObject target;
    public GameObject agent;
    public GameObject rootPlatform;
    public int numOfRepeatedLvl = 1;
    public bool walls = false;
    private List<GameObject> _platformList;
    // private MyPlayerPrefs.PlayerStats _playerStats;
    private int _lvlCounter;

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
        if (_platformList is null)
        {
            _platformList = new List<GameObject>();
        }
        MyPlayerPrefs.PlayerStats _playerStats = MyPlayerPrefs.GetInstance();
        uint level = _playerStats.GetLevel();
        Const.Direction prevDir = Const.Direction.Up;
        GameObject parentPlatform = rootPlatform;
        parentPlatform.GetComponent<PlatformWalls>().SetWallsActive(walls);
        parentPlatform.GetComponent<PlatformWalls>().ResetWalls();
        ResetLevel();

        for (int i = 0; i < level; i++)
        {
            GameObject platform = Instantiate(platformPrefab,gameObject.transform);
            platform.transform.position = new Vector3(0, 0, 0);
            _platformList.Add(platform);
            
            PlatformWalls parentPlatformWalls = parentPlatform.GetComponent<PlatformWalls>();
            PlatformWalls childPlatformWalls = platform.GetComponent<PlatformWalls>();
            childPlatformWalls.SetWallsActive(walls);
            
            
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
            
            parentPlatformWalls.WallHandle(newDir,false);
            switch (newDir)
            {
                case Const.Direction.Up:
                    childPlatformWalls.WallHandle(Const.Direction.Down, false);
                    platformPos.z += Const.PlatformSize;
                    break;
                case Const.Direction.Left:
                    childPlatformWalls.WallHandle(Const.Direction.Right, false);
                    platformPos.x -= Const.PlatformSize;
                    break;
                case Const.Direction.Right:
                    childPlatformWalls.WallHandle(Const.Direction.Left, false);
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
        if (_lvlCounter == MyPlayerPrefs.GetInstance().level)
        {
            MyPlayerPrefs.GetInstance().SetNextLevel();
            _lvlCounter = 0;
        }
        else{
            _lvlCounter++;
            Debug.Log("Repeated level: "+ _lvlCounter + " " + numOfRepeatedLvl);
        }
        
        SetLevel();
    }
}
