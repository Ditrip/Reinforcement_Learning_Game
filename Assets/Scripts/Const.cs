using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Const
{
    public const float PlatformSize = 10;
    public const float TargetSize = 1;

    public enum Direction
    {
        Left = 0,
        Up = 1,
        Right = 2
    }

    public enum Tags
    {
        AiEnv,
        Target,
        Agent
    }

}
