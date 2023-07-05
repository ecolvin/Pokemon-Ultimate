using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] Vector2Int startCoords;
    public Vector2Int StartCoords {get{return startCoords;}}
    [SerializeField] Vector2Int destCoords;
    public Vector2Int DestCoords {get{return destCoords;}}

    Node startNode;
    Node destNode;
    Node curNode;
    
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();
    Queue<Node> q = new Queue<Node>();

    Vector2Int[] directions = {Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down};
    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        if(gridManager != null)
        {
            grid = gridManager.Grid;            
            startNode = grid[startCoords];
            destNode = grid[destCoords];
        }
    }

    void Start()
    {
        GetNewPath();
    }

    public List<Node> GetNewPath()
    {
        return GetNewPath(startCoords);
    }

    public List<Node> GetNewPath(Vector2Int coords)
    {
        gridManager.ResetNodes();
        BFS(coords);
        return Backtrack();
    }

    void ExploreNeighbors()
    {
        foreach(Vector2Int d in directions)
        {
            Vector2Int searchCoords = curNode.coords + d;
            if(grid.ContainsKey(searchCoords))
            {    
                Node n = grid[curNode.coords + d];         
                if(!reached.ContainsKey(n.coords) && n.isWalkable)
                {                    
                    n.parent = curNode;
                    reached.Add(n.coords, n);
                    q.Enqueue(n);
                }
            }
        }
    }

    void BFS(Vector2Int coords)
    {        
        startNode.isWalkable = true;
        destNode.isWalkable = true;
        
        q.Clear();
        reached.Clear();

        bool isRunning = true;

        q.Enqueue(grid[coords]);
        reached.Add(coords, grid[coords]);

        while(q.Count > 0 && isRunning)
        {
            curNode = q.Dequeue();
            curNode.isExplored = true;
            ExploreNeighbors();
            if(curNode.coords == destCoords)
            {
                isRunning = false;
            }
        }
    }

    List<Node> Backtrack()
    {
        if(curNode != destNode){return null;}

        List<Node> path = new List<Node>();

        Node n = curNode;
        while(n != null)
        {
            n.isPath = true;
            path.Add(n);
            n = n.parent;
        }

        path.Reverse();
        return path;
    }

    public bool WillBlockPath(Vector2Int coords)
    {
        if(grid.ContainsKey(coords))
        {
            bool walk = grid[coords].isWalkable;
            grid[coords].isWalkable = false;
            List<Node> newPath = GetNewPath();
            grid[coords].isWalkable = walk;

            if(newPath == null)
            {
                GetNewPath();
                return true;
            }
        }

        return false;
    }

    public void NotifyReceivers()
    {
        BroadcastMessage("RecalculatePath", false, SendMessageOptions.DontRequireReceiver);
    }
}
