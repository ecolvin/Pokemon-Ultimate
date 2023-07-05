using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [Tooltip("The size of one unit of the Unity grid snap settings")]
    [SerializeField] int gridTileSize = 1;
    public int GridTileSize{get{return gridTileSize;}}

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    public Dictionary<Vector2Int, Node> Grid {get{return grid;}}

    void Awake() {
        CreateGrid();
    }

    public Node GetNode(Vector2Int coords)
    {
        if(grid.ContainsKey(coords))
        {
            return grid[coords];
        }

        return null;
    }

    public void BlockNode(Vector2Int coords)
    {
        if(grid.ContainsKey(coords))
        {
            grid[coords].isWalkable = false;
        }
    }

    public void ResetNodes()
    {
        foreach(KeyValuePair<Vector2Int, Node> entry in grid)
        {
            entry.Value.parent = null;
            entry.Value.isExplored = false;
            entry.Value.isPath = false;
        }
    }

    public Vector2Int GetCoordsFromPos(Vector3 pos)
    {
        Vector2Int coords = new Vector2Int();
        coords.x = Mathf.RoundToInt(pos.x / gridTileSize);
        coords.y = Mathf.RoundToInt(pos.z / gridTileSize);

        return coords;
    }

    public Vector3 GetPosFromCoords(Vector2Int coords)
    {
        Vector3 pos = new Vector3();
        pos.x = coords.x * gridTileSize;
        pos.z = coords.y * gridTileSize;

        return pos;
    }

    void CreateGrid()
    {
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int z = 0; z < gridSize.y; z++)
            {
                Vector2Int coords = new Vector2Int(x,z);
                grid.Add(coords, new Node(coords, true));
                //Debug.Log(grid[coords].coords + " = " + grid[coords].isWalkable);
            }
        }
    }
}
