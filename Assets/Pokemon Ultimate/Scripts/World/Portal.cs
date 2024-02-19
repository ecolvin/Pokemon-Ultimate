using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTrigger
{
    [SerializeField] int destinationScene = -1;
    [SerializeField] PortalLink link;
    public PortalLink Link {get => link;}
    [SerializeField] Transform spawnPoint;
    public Transform SpawnPoint {get => spawnPoint;}

    PlayerController player;
    Fader fader;

    public IEnumerator OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        yield return SwitchScene(); 
    }

    void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        yield return fader.Fade(1f);
        yield return SceneManager.LoadSceneAsync(destinationScene);

        Portal destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.Link == this.link);
        player.Character.SnapToPos(destPortal.SpawnPoint.position);
  
        yield return new WaitForSeconds(1f);
        yield return fader.Fade(0f);

        Destroy(gameObject);
    }
}

public enum PortalLink {A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z}
