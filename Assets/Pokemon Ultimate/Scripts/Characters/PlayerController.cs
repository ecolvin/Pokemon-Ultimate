using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour, ISaveable
{
    [SerializeField] Transform body;    
    [SerializeField] Sprite sprite;
    public Sprite Sprite {get => sprite;}

    public event Action<Pokemon> OnEncounter;
    //public event Action<Collider> OnTrainerBattle;

    Fader fader;
    Character character;
    public Character Character {get => character;}

    bool isMoving = false;

    void Awake()
    {
        character = GetComponent<Character>();
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    void Start()
    {        
        fader = FindObjectOfType<Fader>();
    }

    public void HandleUpdate()
    {
        MovePlayer();    
        if(!isMoving && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))   
        {
            Interact();
        } 
    }

    void MovePlayer()
    {
        if(!isMoving)
        {
            StartCoroutine(Move());
        }
    }

    Vector3 getPosLookingAt()
    {
        Vector3 curPos = transform.position;
        int faceDir = (int)body.transform.rotation.eulerAngles.y;
        Vector3 targetPos = curPos;
        if(faceDir == 0)
        {
            targetPos = targetPos + new Vector3(0, 0, 1*GlobalSettings.Instance.GridSize);
        }
        else if(faceDir == 90)
        {
            targetPos = targetPos + new Vector3(1*GlobalSettings.Instance.GridSize, 0, 0);
        }
        else if(faceDir == 180)
        {
            targetPos = targetPos + new Vector3(0, 0, -1*GlobalSettings.Instance.GridSize);
        }
        else if(faceDir == 270)
        {
            targetPos = targetPos + new Vector3(-1*GlobalSettings.Instance.GridSize, 0, 1);
        }
        return targetPos;
    }

    void Interact()
    {
        Vector3 targetPos = getPosLookingAt();

        Collider[] interactables = Physics.OverlapBox(targetPos, new Vector3(GlobalSettings.Instance.GridSize/2, .5f, GlobalSettings.Instance.GridSize/2), Quaternion.identity, GameLayers.Instance.InteractableLayer);
        if(interactables.Length != 0)
        {
            StartCoroutine(interactables[0].GetComponent<Interactable>()?.Interact(transform.position));
        } 
    }

    IEnumerator Move()
    {        
        isMoving = true;
        
        Vector3 curPos = transform.position;
        float xOffset = Input.GetAxisRaw("Horizontal") * GlobalSettings.Instance.GridSize;
        float zOffset = Input.GetAxisRaw("Vertical") * GlobalSettings.Instance.GridSize;
        Vector3 newPos = new Vector3(curPos.x + xOffset, curPos.y, curPos.z + zOffset);
        if(newPos == curPos)
        {
            isMoving = false;
            yield break;
        }

        yield return character.SmoothGridMovement(newPos);


        Collider[] triggers = Physics.OverlapBox(transform.position, new Vector3(GlobalSettings.Instance.GridSize/4, .5f, GlobalSettings.Instance.GridSize/4), Quaternion.identity, GameLayers.Instance.TriggerLayers);
        
        bool waitForTriggers = false;

        foreach(var trig in triggers)
        {
            IPlayerTrigger trigger = trig.GetComponent<IPlayerTrigger>();
            if(trigger != null)
            {
                trigger.OnPlayerTriggered(this);
                waitForTriggers = true;
                break;
            }
        }      

        if(!waitForTriggers)
        {
            isMoving = false;
        }
    }

    public void EndTrigger()
    {
        isMoving = false;
    }

    public IEnumerator TriggerEncounter(Pokemon p)
    {
        yield return fader.BattleStart();
        yield return fader.Fade(1);
        yield return new WaitForSeconds(.5f);
        OnEncounter(p);   //Shiny, HA, Perfect IVs
        yield return fader.Fade(0);
        isMoving = false;
    }

    public void LoadData(GameData data)
    {
        PlayerSaveData saveData = data.player;
        if(saveData != null)
        {
            transform.position = saveData.position;
            body.transform.rotation = saveData.rotation;
            GetComponent<Party>().Pokemon = saveData.pokemon.Select(s => new Pokemon(s)).ToList();
            GetComponent<Inventory>().SetInventory(saveData.inventory);
        }
    }

    public void SaveData(ref GameData data)
    {
        List<Tab> inv = new List<Tab>();
        foreach(KeyValuePair<InventoryTab, List<ItemSlot>> pair in Inventory.GetInventory().Inv)
        {
            Tab t = new Tab()
            {
                tab = pair.Key,
                items = pair.Value
            };

            inv.Add(t);
        }

        PlayerSaveData saveData = new PlayerSaveData()
        {            
            position = transform.position,
            rotation = body.transform.rotation,
            pokemon = GetComponent<Party>().Pokemon.Select(p => p.GetSaveData()).ToList(),
            inventory = inv
        };

        data.player = saveData;
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public Vector3 position;
    public Quaternion rotation;
    public List<PokemonSaveData> pokemon;
    public List<Tab> inventory = new List<Tab>();
}