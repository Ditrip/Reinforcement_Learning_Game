using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PillarGen : MonoBehaviour
{

    public GameObject pillarPrefab;
    [Tooltip("Number of pillar 1-9")]
    [Range(1,9)]
    public uint numOfPillars = 5;

    private List<GameObject> _pillarList;
    private List<SpawnRange> _pillarSpawnPos;

    struct SpawnRange
    {
        public SpawnRange(float minX, float maxX, float minZ, float maxZ)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minZ = minZ;
            this.maxZ = maxZ;
        }
        public float minX { get; }
        public float maxX { get; }
        public float minZ { get; }
        public float maxZ { get; }
    }
    private void Start()
    {
        SetPillars();
    }

    public void SetPillars()
    {
        _pillarList ??= new List<GameObject>();
        _pillarSpawnPos = GetSpawnPos(); 
        
        foreach(SpawnRange spawnRange in _pillarSpawnPos)
        {
            GameObject pillar = Instantiate(pillarPrefab, gameObject.transform);
            Vector3 spawnPos = gameObject.transform.position;
            float posX = Random.Range(spawnRange.minX, spawnRange.maxX) - Const.PlatformSize/2;
            float posZ = Random.Range(spawnRange.minZ, spawnRange.maxZ) - Const.PlatformSize/2;

            Vector3 pillarPos = new Vector3(posX, Const.PillarHeight, posZ);
            Vector3 position = spawnPos + pillarPos;
            pillar.transform.position = position;
            
            _pillarList.Add(pillar);
        }
    }

    private List<SpawnRange> GetSpawnPos()
    {
        uint numOfCol = 0;
        uint numOfRow = 0;
        
        numOfPillars = numOfPillars == 0 ? 1 : numOfPillars;
        if (numOfPillars == 1)
        {
            numOfCol = 1;
            numOfRow = 1;
        }
        else
        {
            numOfCol = (uint)Math.Ceiling(Math.Sqrt(numOfPillars));
            // Debug.Log("Num of columns: " + numOfCol);
            numOfRow = (uint)Math.Ceiling((float)numOfPillars / numOfCol);
            // Debug.Log("Num of rows: " + numOfRow);
        }

        List<SpawnRange> pillarSpawnPos = new List<SpawnRange>();
        uint numOfObj = numOfPillars;

        for (int row = 0; row < numOfRow; row++)
        {
            float minX = 0, maxX = 0;
            float minZ = 0, maxZ = 0;

            minZ = (Const.PlatformSize / numOfRow) * row;
            minZ = minZ == 0 ? 1 : minZ;
            maxZ = (Const.PlatformSize / numOfRow) * (row+1);
            maxZ = Math.Abs(maxZ - Const.PlatformSize) < 0.3f ? 9 : maxZ; 
            if (numOfObj >= numOfCol)
            {
                for (uint col = 0; col < numOfCol; col++)
                {
                    minX = (Const.PlatformSize / numOfCol) * col;
                    minX = minX == 0 ? 1 : minX;
                    maxX = (Const.PlatformSize / numOfCol) * (col+1);
                    maxX = Math.Abs(maxX - Const.PlatformSize) < 0.3f ? 9 : maxX;
                    pillarSpawnPos.Add(new SpawnRange(minX,maxX,minZ,maxZ));
                }
            }
            else
            {
                for (uint col = 0; col < numOfObj; col++)
                {
                    minX = (Const.PlatformSize / numOfCol) * col;
                    minX = minX == 0 ? 1 : minX;
                    maxX = (Const.PlatformSize / numOfCol) * (col+1);
                    maxX = Math.Abs(maxX - Const.PlatformSize) < 0.3f ? 9 : maxX;
                    pillarSpawnPos.Add(new SpawnRange(minX,maxX,minZ,maxZ));
                }
            }
            
            numOfObj -= numOfCol;
        }

        return pillarSpawnPos;
    }

    public void ResetPillars()
    {
        foreach (GameObject pillar in _pillarList)
        {
            Destroy(pillar);
        }
        SetPillars();
    }
}
