using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpPortal : MonoBehaviour, IPlayerTrigger
{
    [SerializeField] PortalLink link;
    public PortalLink Link {get => link;}
    [SerializeField] Transform spawnPoint;
    public Transform SpawnPoint {get => spawnPoint;}

    PlayerController player;
    Fader fader;

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(Warp()); 
    }

    void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator Warp()
    {
        yield return fader.Fade(1f);

        WarpPortal destPortal = FindObjectsOfType<WarpPortal>().First(x => x != this && x.Link == this.link);
        player.Character.SnapToPos(destPortal.SpawnPoint.position);
  
        yield return new WaitForSeconds(1f);
        yield return fader.Fade(0f);

        player.EndTrigger();
    }
}
