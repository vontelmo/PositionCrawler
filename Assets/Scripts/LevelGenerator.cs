using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{


    private int[] floorPlan;
    private int floorPlanCount;
    private int minRooms;
    private int maxRooms;
    private List<int> endRooms;
    private int bossRoomIndex;
    private int secretRoomIndex;
    private int shopRoomIndex;
    private int itemRoomIndex;
    public Cell cellPrefab;
    private float cellSize;
    private Queue<int> cellQueue;
    private List<Cell> spawnedCells;


    [Header("Sprite References")]
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite shop;
    [SerializeField] private Sprite boss;
    [SerializeField] private Sprite secret;

    // Start is called before the first frame update

    void Start()
    {
        minRooms = 7; 
        maxRooms = 15;
        cellSize = 0.5f;
        spawnedCells = new();

        SetupDungeon();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SetupDungeon();
        }
    }

    void SetupDungeon()
    {
        for (int i = 0; i < spawnedCells.Count; i++)
        {
            Destroy(spawnedCells[i].gameObject);
        }
        spawnedCells.Clear();

        floorPlan = new int[100];
        floorPlanCount = default;
        cellQueue  = new Queue<int>();
        endRooms = new List<int>();
        VisitCell(45);
        GenerateDungeon();
    }


    void GenerateDungeon()
    {
        while (cellQueue.Count > 0)
        {
            int index = cellQueue.Dequeue();
            int x = index % 10;

            bool created = false;
            
            if (x > 1) created |= VisitCell(index - 1);
            if (x < 9) created |= VisitCell(index + 1);
            if (index > 20) created |= VisitCell(index - 10);
            if (index < 70) created |= VisitCell(index + 10);
            
            if (created == false)
            {
                endRooms.Add(index);
            }

        }
        
        if (floorPlanCount < minRooms)
        {
            SetupDungeon();
            return;
        }

        SetupSpecialRooms();

    }
    

    
    void SetupSpecialRooms()
    {

    }
        
    void UpdateSpecialRoomsVisuals()
    {

    }

    int PickSecretRoom()
    {
        return -1;
    }


    private int GetNeightbourCount(int index)
    {
        return floorPlan[index - 10] + floorPlan[index - 1] + floorPlan[index + 1] + floorPlan[index + 10];
    }


    private bool VisitCell(int index)
    {
        if (floorPlan[index] != 0 || GetNeightbourCount(index) > 1 || floorPlanCount > maxRooms || UnityEngine.Random.value < 0.5f)
        {
            return false;
        }
        cellQueue.Enqueue(index);
        floorPlan[index] = 1;
        floorPlanCount++;
        SpawnRoom(index);
        return true;
    }


    private void SpawnRoom(int index)
    {
        int x = index % 10;
        int y = index / 10;
        Vector2 position = new Vector2(x * cellSize, -y * cellSize);

        Cell newCell = Instantiate(cellPrefab, position, Quaternion.identity); 

        newCell.value = 1;
        newCell.index = index;

        spawnedCells.Add(newCell);
    }

}
