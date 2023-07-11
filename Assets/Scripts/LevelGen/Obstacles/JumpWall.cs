using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpWall : MonoBehaviour
{
    public void SetJumpWall(Const.Direction wallDir)
    {
        PlatformWalls platformWalls = gameObject.GetComponent<PlatformWalls>();
        platformWalls.WallHandle(wallDir,true);
        platformWalls.GetWallFromDir(wallDir).tag = Const.Tags.JumpWall.ToString();
    }
}
