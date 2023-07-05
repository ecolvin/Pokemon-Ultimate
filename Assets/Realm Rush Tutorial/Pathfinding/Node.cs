using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Vector2Int coords;
    public bool isWalkable;
    public bool isExplored;
    public bool isPath;
    public Node parent;

    public Node(Vector2Int coordinates, bool walkable)
    {
        coords = coordinates;
        isWalkable = walkable;
        parent = null;
    }
}
