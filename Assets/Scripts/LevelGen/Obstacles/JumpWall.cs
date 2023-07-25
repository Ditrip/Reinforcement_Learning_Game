using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JumpWall : MonoBehaviour
{
    public GameObject jumpWallPrefab;
    [Tooltip("Range 1-9")]
    [Range(Const.AgentSize*1.5f,9.0f)]
    public float gap;

    private GameObject leftWall;
    private GameObject rightWall;

    public void SetJumpWall(Const.Direction wallDir)
    {
        if (leftWall is not null)
            Destroy(leftWall);
        if (rightWall is not null)
            Destroy(rightWall);
        PlatformWalls platformWalls = gameObject.GetComponent<PlatformWalls>();
        platformWalls.WallHandle(wallDir,true);
        platformWalls.SetWallHeight(wallDir,Const.WallHeight.Normal);
        platformWalls.GetWallFromDir(wallDir).tag = Const.Tags.JumpWall.ToString();
        SetWall(wallDir);
    }

    private void SetWall(Const.Direction wallDir)
    {
        leftWall = Instantiate(jumpWallPrefab, gameObject.transform);
        rightWall = Instantiate(jumpWallPrefab, gameObject.transform);
        Vector3 parentWallPos = gameObject.GetComponent<PlatformWalls>().GetWallFromDir(wallDir).transform.position;
        float gapPos = Random.Range(gap/2, Const.PlatformSize-gap/2);

        leftWall.transform.position = parentWallPos;
        rightWall.transform.position = parentWallPos;
        leftWall.tag = Const.Tags.JumpWall.ToString();
        rightWall.tag = Const.Tags.JumpWall.ToString();

        float leftWallScale = gapPos - gap/2, 
            rightWallScale = Const.PlatformSize - (gapPos + gap/2);
        Vector3 vector3;

        switch (wallDir)
        {
            case Const.Direction.Left:
            case Const.Direction.Right:
                vector3 = leftWall.transform.localScale;
                vector3.z = leftWallScale;
                vector3.y = (float)Const.WallHeight.High;
                leftWall.transform.localScale = vector3;
                
                vector3 = rightWall.transform.localScale;
                vector3.z = rightWallScale;
                vector3.y = (float)Const.WallHeight.High;
                rightWall.transform.localScale = vector3;

                vector3 = leftWall.transform.position;
                vector3.z += -Const.PlatformSize/2 + (gapPos - gap/2)/2;
                vector3.y = gameObject.transform.position.y + (float)Const.WallHeight.High / 2;
                leftWall.transform.position = vector3;
                
                vector3 = rightWall.transform.position;
                vector3.z += (gapPos + gap/2) / 2;
                vector3.y = gameObject.transform.position.y + (float)Const.WallHeight.High / 2;
                rightWall.transform.position = vector3;
                break;
            case Const.Direction.Up:
            case Const.Direction.Down:
                vector3 = leftWall.transform.localScale;
                vector3.x = leftWallScale;
                vector3.y = (float)Const.WallHeight.High;
                leftWall.transform.localScale = vector3;
                
                vector3 = rightWall.transform.localScale;
                vector3.x = rightWallScale;
                vector3.y = (float)Const.WallHeight.High;
                rightWall.transform.localScale = vector3;

                vector3 = leftWall.transform.position;
                vector3.x += -Const.PlatformSize/2 + (gapPos - gap/2)/2;
                vector3.y = gameObject.transform.position.y + (float)Const.WallHeight.High / 2;
                leftWall.transform.position = vector3;
                
                vector3 = rightWall.transform.position;
                vector3.x += (gapPos + gap/2) / 2;
                vector3.y = gameObject.transform.position.y + (float)Const.WallHeight.High / 2;
                rightWall.transform.position = vector3;
                break;
            default:
                leftWallScale = 0;
                rightWallScale = 0;
                Debug.LogWarning("JumpWall error (wall scale set to default)(Dir: " + wallDir + ")");
                break;
        }
        
        

    }
}
