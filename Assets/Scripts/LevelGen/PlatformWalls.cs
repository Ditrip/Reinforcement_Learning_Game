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
        if (!_isActive)
            return;
        
        switch (wallDir)
        {
            case Const.Direction.Up:
                wallUp.SetActive(activate);
                break;
            case Const.Direction.Down:
                wallDown.SetActive(activate);
                break;
            case Const.Direction.Left:
                wallLeft.SetActive(activate);
                break;
            case Const.Direction.Right:
                wallRight.SetActive(activate);
                break;
        }
    }

    public void ResetWalls()
    {
        if(!_isActive)
            return;
        wallUp.SetActive(true);
        wallDown.SetActive(true);
        wallLeft.SetActive(true);
        wallRight.SetActive(true);
    }

    public void SetWallsActive(bool isActive)
    {
        _isActive = isActive;
        wallUp.SetActive(isActive);
        wallDown.SetActive(isActive);
        wallLeft.SetActive(isActive);
        wallRight.SetActive(isActive);
    }
}
