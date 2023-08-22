using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Const
{
    public const float PlatformSize = 10;
    public const float AgentSize = 1;
    public const float TargetSize = 2;
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
        RootPlatform,
        NextPlatform,
        JumpWall,
        TrainAiEnv,
        Obstacle,
        PrevPlatform,
        LeftJumpWall,
        RightJumpWall
    }

    public enum Platforms
    {
        Default = 0,
        JumpWall = 1,
        Pillars = 2
        // FallingObj = 3,
        // FadePlatform = 4
    }

    public enum WallHeight
    {
        Normal = 1,
        High = 3
    }
    
    public static Direction GetOppositeDirection(Direction dir)
    {
        Direction opDir;
        switch (dir)
        {
            case Direction.Left:
                opDir = Direction.Right;
                break;
            case Direction.Up:
                opDir = Direction.Down;
                break;
            case Direction.Right:
                opDir = Direction.Left;
                break;
            case Direction.Down:
                opDir = Direction.Up;
                break;
            default:
                Debug.LogError("Const.cs (GetOppositeDirection): returned default direction (Up)");
                opDir = Direction.Up;
                break;
        }

        return opDir;
    }

}
