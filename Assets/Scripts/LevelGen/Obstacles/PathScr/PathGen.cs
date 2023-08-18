using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathGen : MonoBehaviour
{

    public GameObject targetPrefab;
    public GameObject pathPrefab;

    private GameObject _targetObj;
    private List<GameObject> _pathObjs;
    private TextMeshProUGUI _targetText;
    private List<TextMeshProUGUI> _pathTexts;
    private int _target;

    // Use Update method in Scene 'TestObstacles' to check if the script works properly
    // private int _counter = 0;
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         GameObject parentPlatform = GameObject.Find("ParentPlatform"); 
    //         parentPlatform.GetComponent<PlatformWalls>().WallHandle(Const.Direction.Up,false);
    //         SetPlatform(parentPlatform,Const.Direction.Up,(Const.Direction)Enum.GetValues(typeof(Const.Direction)).GetValue(_counter%3));
    //         _counter++;
    //     }
    // }

    public void SetPlatform(GameObject parentPlatform,Const.Direction parentDir, Const.Direction dir)
    {
        _pathObjs ??= new List<GameObject>();
        _pathTexts ??= new List<TextMeshProUGUI>();
        
        SetTarget(parentPlatform,parentDir);
        SetPaths(dir);
    }

    private void SetTarget(GameObject parentPlatform, Const.Direction dir)
    {
        _target = Random.Range(1, Const.PathsNum+1);
        _targetObj ??= Instantiate(targetPrefab,parentPlatform.transform);
        _targetText ??= _targetObj.GetComponentInChildren<TextMeshProUGUI>();
        _targetText.text = ""+_target;
        _targetObj.tag = Enum.GetNames(typeof(Const.TagsPathTarget))[_target-1];

        Vector3 targetPos = _targetObj.transform.localPosition;
        targetPos.y = _targetObj.transform.localScale.y/2;
        _targetObj.transform.localPosition = targetPos;
        Quaternion transformRotation = _targetObj.transform.rotation;
        switch (dir)
        {
            case Const.Direction.Left:
                transformRotation.eulerAngles = new Vector3(0, (int)Const.Rotation.Left, 0);
                break;
            case Const.Direction.Right:
                transformRotation.eulerAngles = new Vector3(0, (int)Const.Rotation.Right, 0);
                break;
            default:
                transformRotation.eulerAngles = new Vector3(0, (int)Const.Rotation.Default, 0);
                break;
        }
        _targetObj.transform.rotation = transformRotation;

    }

    private void SetPaths(Const.Direction dir)
    {
        PlatformWalls platformWalls = gameObject.GetComponent<PlatformWalls>();
        platformWalls.ResetWalls();
        platformWalls.WallHandle(dir, false);
        platformWalls.WallHandle(Const.GetOppositeDirection(dir), false);
        DestroyPaths();
        for (int i = 0; i < Const.PathsNum; i++)
        {
            GameObject pathObj = Instantiate(pathPrefab, gameObject.transform);
            _pathObjs.Add(pathObj);
            PathScr pathScr = pathObj.GetComponentInChildren<PathScr>();
            pathScr.gameObject.tag = Enum.GetNames(typeof(Const.TagsPathGoal))[i];
            TextMeshProUGUI text = pathObj.GetComponentInChildren<TextMeshProUGUI>();
            _pathTexts.Add(text);
            text.text = "" + (i + 1);

            Vector3 pos = pathObj.transform.localPosition;
            Quaternion quaternion = pathObj.transform.rotation;;
            switch (dir)
            {
                case Const.Direction.Up:
                    pos.z = -Const.PlatformSize / 2;
                    pos.x = -Const.PlatformSize/Const.PathsNum + (i * (Const.PlatformSize / Const.PathsNum));
                    break;
                case Const.Direction.Left:
                    pos.x = Const.PlatformSize / 2;
                    pos.z = -Const.PlatformSize/Const.PathsNum + (i * (Const.PlatformSize / Const.PathsNum));
                    quaternion.eulerAngles = new Vector3(0, (float)Const.Rotation.Left, 0);
                    break;
                case Const.Direction.Right:
                    pos.x = -Const.PlatformSize / 2;
                    pos.z = Const.PlatformSize/Const.PathsNum - (i * (Const.PlatformSize / Const.PathsNum));
                    quaternion.eulerAngles = new Vector3(0, (float)Const.Rotation.Right, 0);
                    break;
            }
            pathObj.transform.localPosition = pos;
            pathObj.transform.rotation = quaternion;
        }
        
        _pathObjs[_target-1].GetComponentInChildren<PathScr>().SetPathPass(true);
        
    }

    private void DestroyPaths()
    {
        _pathTexts.Clear();
        foreach (GameObject pathObj in _pathObjs)
        {
            Destroy(pathObj);
        }
        _pathObjs.Clear();
    }

    public void OnDestroy()
    {
        Destroy(_targetObj);
    }
}
