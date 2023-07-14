using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Const
{
    public const float PlatformSize = 10;
    public const float AgentSize = 1;
    public const float TargetSize = 1;
    public const float PillarHeight = 3;

    public enum Direction
    {
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3
    }

    public enum Tags
    {
        AiEnv,
        Target,
        Agent,
        Platform,
        Wall,
        FallingObj,
        RootPlatform,
        NextPlatform,
        JumpWall,
        TrainAiEnv
    }

    public enum Platforms
    {
        FallingObj,
        FadePlatform,
        JumpWall,
        Pillars,
        Default
    }

    public enum WallHeight
    {
        Normal = 1,
        High = 3
    }
}
