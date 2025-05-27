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

    // Evento que se dispara cuando se genera un nuevo dungeon
    public static event Action OnDungeonGenerated;


    public Cell cellPrefab;
    private float cellSize;
    private Queue<int> cellQueue;
    private List<Cell> spawnedCells;
    public int[] FloorPlan => floorPlan;
    public List<Cell> SpawnedCells => spawnedCells;

    [Header("Sprite References")]
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite shop;
    [SerializeField] private Sprite boss;
    [SerializeField] private Sprite secret;

    // Start is called before the first frame update

    void Awake()
    {
        minRooms = 30; 
        maxRooms = 40;
        cellSize = 0.6f;
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
        VisitCell(45);
        GenerateDungeon();

        OnDungeonGenerated?.Invoke();
    }


    void GenerateDungeon()
    {
        while (cellQueue.Count > 0)
        {
            int index = cellQueue.Dequeue();
            int x = index % 10;

            // Intentar crear habitaciones en todas las direcciones adyacentes válidas
            if (x > 0) VisitCell(index - 1);         // Izquierda
            if (x < 9) VisitCell(index + 1);         // Derecha
            if (index >= 10) VisitCell(index - 10);  // Arriba
            if (index < 90) VisitCell(index + 10);   // Abajo
        }

        // Si se generaron muy pocas habitaciones, reiniciar
        if (floorPlanCount < minRooms)
        {
            SetupDungeon();
            return;
        }
    }



    private int GetNeighbourCount(int index)
    {
        int count = 0;

        int width = 10; // ancho del mapa
        int height = 10; // alto del mapa
        int totalSize = width * height;

        int x = index % width;
        int y = index / width;

        // Arriba
        if (y > 0) count += floorPlan[index - width];
        // Abajo
        if (y < height - 1) count += floorPlan[index + width];
        // Izquierda
        if (x > 0) count += floorPlan[index - 1];
        // Derecha
        if (x < width - 1) count += floorPlan[index + 1];

        return count;
    }



    private bool VisitCell(int index)
    {
        if (floorPlan[index] != 0 || GetNeighbourCount(index) > 2 || floorPlanCount > maxRooms || UnityEngine.Random.value < 0.3f)
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
