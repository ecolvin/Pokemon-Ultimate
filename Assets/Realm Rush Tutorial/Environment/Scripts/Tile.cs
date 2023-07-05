using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] Tower towerPrefab;
    
    [SerializeField] bool isPlaceable;    
    public bool IsPlaceable{get{return isPlaceable;}}

    GridManager gridManager;
    Pathfinding pathfinder;
    Vector2Int coords = new Vector2Int();

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinder = FindObjectOfType<Pathfinding>();
    }

    void Start()
    {
        if(gridManager != null)
        {
            coords = gridManager.GetCoordsFromPos(transform.position);

            if(!isPlaceable)
            {
                gridManager.BlockNode(coords);
            }
        }
    }


    void OnMouseDown()
    {
        if(gridManager.GetNode(coords).isWalkable && !pathfinder.WillBlockPath(coords))
        {           
            bool successful = towerPrefab.CreateTower(towerPrefab, transform.position);
            if(successful)
            {
                gridManager.BlockNode(coords);
                pathfinder.NotifyReceivers();
            }
        }
    }
}
