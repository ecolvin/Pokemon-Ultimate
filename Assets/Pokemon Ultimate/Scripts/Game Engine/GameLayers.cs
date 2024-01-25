using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask obstacleLayer;
    public LayerMask ObstacleLayer {get => obstacleLayer;}
    [SerializeField] LayerMask interactableLayer;
    public LayerMask InteractableLayer {get => interactableLayer;}
    [SerializeField] LayerMask spawnerLayer;
    public LayerMask SpawnerLayer {get => spawnerLayer;}
    [SerializeField] LayerMask playerLayer;
    public LayerMask PlayerLayer {get => playerLayer;}
    [SerializeField] LayerMask portalLayer;
    public LayerMask PortalLayer {get => portalLayer;}

    public LayerMask BlockingLayers {get => obstacleLayer | interactableLayer | playerLayer;}
    public LayerMask TriggerLayers {get => spawnerLayer | portalLayer;}
    public static GameLayers Instance {get; set;}
    
    void Awake()
    {
        Instance = this;
    }

}
