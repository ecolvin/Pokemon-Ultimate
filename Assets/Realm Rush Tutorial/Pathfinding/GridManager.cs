using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    void Awake() {
        CreateGrid();
    }

    void CreateGrid()
    {
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int z = 0; z < gridSize.y; z++)
            {
                Vector2Int coords = new Vector2Int(x,z);
                grid.Add(coords, new Node(coords, true));
                Debug.Log(grid[coords].coords + " = " + grid[coords].isWalkable);
            }
        }
    }
}
