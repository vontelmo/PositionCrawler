using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public float cellSize = 0.6f;
    public int gridWidth = 10; // ancho del mapa
    private Vector2 currentPosition;
    private bool canMove = true;

    private LevelGenerator generator;

    void OnEnable()
    {
        LevelGenerator.OnDungeonGenerated += ReactToDungeon;
    }

    void OnDisable()
    {
        LevelGenerator.OnDungeonGenerated -= ReactToDungeon;
    }

    void ReactToDungeon()
    {
        Debug.Log("¡Nuevo dungeon generado!");
        MoveTo( generator.SpawnedCells[0].transform.position);

    }

    void Awake()
    {
        generator = FindObjectOfType<LevelGenerator>();
    }

    void Update()
    {
        if (!canMove) return;

        Vector2 input = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W)) input = Vector2.up;
        if (Input.GetKeyDown(KeyCode.S)) input = Vector2.down;
        if (Input.GetKeyDown(KeyCode.A)) input = Vector2.left;
        if (Input.GetKeyDown(KeyCode.D)) input = Vector2.right;

        if (input != Vector2.zero)
        {
            Vector2 targetPosition = currentPosition + input * cellSize;

            int index = GetIndexFromPosition(targetPosition);

            if (index >= 0 && index < generator.FloorPlan.Length && generator.FloorPlan[index] == 1)
            {
                MoveTo(targetPosition);
            }
            else
            {
                Debug.Log("Habitación no existente o fuera del mapa");
            }
        }
    }

    int GetIndexFromPosition(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / cellSize);
        int y = Mathf.RoundToInt(-position.y / cellSize); // Y negativo porque tu generador usa -y

        return y * gridWidth + x;
    }

    void MoveTo(Vector2 targetPosition)
    {
        currentPosition = targetPosition;
        transform.position = currentPosition;
    }
}
