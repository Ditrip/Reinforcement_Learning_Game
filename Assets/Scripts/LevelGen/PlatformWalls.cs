using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformWalls : MonoBehaviour
{
    public GameObject wallUp;
    public GameObject wallDown;
    public GameObject wallLeft;
    public GameObject wallRight;

    private bool _isActive;
    private void Awake()
    {
        _isActive = true;
    }

    public void WallHandle(Const.Direction wallDir, bool activate)
    {
        GetWallFromDir(wallDir).SetActive(activate);
    }

    public void ResetWalls()
    {
        wallUp.SetActive(_isActive);
        wallDown.SetActive(_isActive);
        wallLeft.SetActive(_isActive);
        wallRight.SetActive(_isActive);
    }

    public void SetWallsActive(bool isActive)
    {
        _isActive = isActive;
        wallUp.SetActive(isActive);
        wallDown.SetActive(isActive);
        wallLeft.SetActive(isActive);
        wallRight.SetActive(isActive);
    }

    public GameObject GetWallFromDir(Const.Direction wallDir)
    {
        GameObject wall;
        switch (wallDir)
        {
            case Const.Direction.Down:
                wall = wallDown;
                break;
            case Const.Direction.Left:
                wall = wallLeft;
                break;
            case Const.Direction.Right:
                wall = wallRight;
                break;
            case Const.Direction.Up:
                wall = wallUp;
                break;
            default:
                wall = null;
                break;
        }
        if(wall is null)
            Debug.LogWarning("PlatformWalls(GetWallFromDir) no wall returned (null)");

        return wall;
    }

    public void SetHeight(Const.WallHeight wallHeight)
    {
        if (!_isActive)
            return;
        foreach (Const.Direction dir in Enum.GetValues(typeof(Const.Direction)))
        {
            SetWallHeightPos(dir,wallHeight);
        }
    }

    public void SetWallHeight(Const.Direction dir, Const.WallHeight wallHeight)
    {
        SetWallHeightPos(dir,wallHeight);
    }

    private void SetWallHeightPos(Const.Direction dir, Const.WallHeight wallHeight)
    {
        GameObject wall = GetWallFromDir(dir);
        Vector3 wallPos = wall.transform.localPosition;
        Vector3 wallScale = wall.transform.localScale;
        switch (wallHeight)
        {
            case Const.WallHeight.High:
                wallPos.y = 1.5f;
                wallScale.y = (float) Const.WallHeight.High;
                break;
            case Const.WallHeight.Normal:
                wallPos.y = 0.5f;
                wallScale.y = (float)Const.WallHeight.Normal;
                break;
        }
        wall.transform.localPosition = wallPos;
        wall.transform.localScale = wallScale;
    }
}
