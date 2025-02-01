using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;

public class Statistic
{

    private static Statistic _instance;
    private static readonly string[] LOGFiles = new[] { "logAvgStepsPerLvl.txt", "logAvgPlatformsPerLvl.txt" };
    private string _path;
    private int _currentLvl;
    private readonly List<int> _numOfSteps;
    private readonly List<LevelScr.ObstNum> _numOfObstacles;
    private readonly StatsRecorder _recorder = Academy.Instance.StatsRecorder;

    public static Statistic GetInstance()
    {
        return _instance ??= new Statistic();
    }

    private Statistic()
    {
        SetFiles();
        _numOfSteps = new List<int>();
        _numOfObstacles = new List<LevelScr.ObstNum>();
        _currentLvl = -1;
    }
    
    private void SetFiles()
    {
        _path = Application.dataPath;
        _path += "/Statistics/";
        foreach (string logFile in LOGFiles)
        {
            if (!File.Exists(_path+logFile))
            {
                File.Create(_path+logFile);
            }
        }
    }

    public void CollectStats(int level, uint steps, LevelScr.ObstNum obstNum)
    {
        if (_currentLvl == -1)
            _currentLvl = level;

        if (_currentLvl != level)
        {
            WriteAvgStepsPerLvl(_currentLvl);
            WriteAvgPlatformPerLvl(_currentLvl);
            _numOfSteps.Clear();
            _numOfObstacles.Clear();
            _currentLvl = level;
        }
        
        _numOfSteps.Add((int)steps);
        _numOfObstacles.Add(obstNum);

    }

    private void WriteAvgStepsPerLvl(int level)
    {
        double avgSteps = _numOfSteps.Average();
        using (StreamWriter sw = File.AppendText(_path+LOGFiles[0]))
        {
            sw.WriteLine("Level: " + level + " Steps: " + avgSteps);
        }
        _recorder.Add("Agent/Avg Steps", (float)avgSteps);
    }

    private void WriteAvgPlatformPerLvl(int level)
    {
        float avgDefault = 0;
        float avgPillar = 0;
        float avgJumpWall = 0;

        foreach (LevelScr.ObstNum obstNum in _numOfObstacles)
        {
            avgDefault += obstNum.DefaultNum;
            avgPillar += obstNum.PillarNum;
            avgJumpWall += obstNum.JumpWallNum;
        }

        float sumOfPlatforms = avgDefault + avgPillar + avgJumpWall;
        avgDefault /= sumOfPlatforms;
        avgPillar /= sumOfPlatforms;
        avgJumpWall /= sumOfPlatforms;
            
        using (StreamWriter sw = File.AppendText(_path+LOGFiles[1]))
        {
            sw.WriteLine("Level: " + level + "\n\tDefault: %" + avgDefault*100 
            + "\n\tPillar: %" + avgPillar*100 + "\n\tJumpWall: %" + avgJumpWall*100);
        }
        _recorder.Add("Platforms/Avg Default Platform Spawn", (float)avgDefault);
        _recorder.Add("Platforms/Avg Pillar Platform Spawn", (float)avgPillar);
        _recorder.Add("Platforms/Avg Jump Wall Platform Spawn", (float)avgJumpWall);
    }
}
