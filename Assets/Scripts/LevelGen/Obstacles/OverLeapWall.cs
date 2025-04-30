using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class OverLeapWall : MonoBehaviour
{
    public GameObject jumpBlockPrefab;
    
    private GameObject _jumpBlock;

    public void SetOverLeapPlatform(Const.Direction wallDir)
    {
        if (_jumpBlock is not null)
            Destroy(_jumpBlock);

        SetJumpBlock(wallDir);
        
    }

    private void SetJumpBlock(Const.Direction wallDir)
    {
        Vector3 parentWallPos = gameObject.GetComponent<PlatformWalls>().GetWallFromDir(wallDir).transform.position;
        _jumpBlock = Instantiate(jumpBlockPrefab, gameObject.transform);
        Vector3 pos = parentWallPos;
        Vector3 scale = _jumpBlock.transform.localScale;
        

        switch (wallDir)
        {
            case Const.Direction.Up:
                pos.z -= Const.PlatformSize / 5;
                scale.z = 3;
                scale.x = Const.PlatformSize / 2;
                
                break;
            case Const.Direction.Left:
                pos.x += Const.PlatformSize / 5;
                scale.x = 3;
                scale.z = Const.PlatformSize / 2;
                break;
            case Const.Direction.Right:
                pos.x -= Const.PlatformSize / 5;
                scale.x = 3;
                scale.z = Const.PlatformSize / 2;
                break;
        }

        _jumpBlock.transform.position = pos;
        scale.y = (float)Const.WallHeight.High/2;
        _jumpBlock.transform.localScale = scale;

    }
}
