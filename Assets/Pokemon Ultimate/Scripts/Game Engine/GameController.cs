using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState {Overworld, Battle, Dialog, Menu, Pokemon, Bag}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Battle battle;
    [SerializeField] PartyScreen pokemonScreen;
    [SerializeField] InventoryUI inventoryUI;

    GameState state;
    GameState savedState;
    //Route curRoute;

    MenuController menuController;

    public SceneDetails CurScene {get; private set;}
    public SceneDetails PrevScene {get; private set;}

    public static GameController Instance {get; private set;}

    void Awake()
    {
        Instance = this;
        menuController = GetComponent<MenuController>();
        
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Start() 
    {
        pokemonScreen.Init();
        player.OnEncounter += StartBattle;    
        battle.OnBattleOver += EndBattle;
        menuController.OnSelection += ExecuteMenuSelection;

        DialogManager.Instance.OnShowDialog += () => 
        {
            if(state == GameState.Overworld)
            {
                savedState = state;
                state = GameState.Dialog;
            }
        };

        DialogManager.Instance.OnCloseDialog += () => 
        {
            if(state == GameState.Dialog)
            {
                state = savedState;
            }
        };

        menuController.OnClose += () =>
        {
            state = GameState.Overworld;
        };
    }

    void Update() 
    {
        if(state == GameState.Overworld)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
            else
            {
                player.HandleUpdate();
            }
        }    
        else if(state == GameState.Battle)
        {
            battle.HandleUpdate();
        }
        else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if(state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if(state == GameState.Pokemon)
        {
            pokemonScreen.HandleInput();
        }
        else if(state == GameState.Bag)
        {
            inventoryUI.HandleUpdate();
        }
    }

    public void SetCurrentScene(SceneDetails newScene)
    {
        PrevScene = CurScene;
        CurScene = newScene;
    }

    void StartBattle(Pokemon p)//int shinyRolls, int haRolls, int numPerfect)
    {
        state = GameState.Battle;
        battle.gameObject.SetActive(true);
        battle.StartBattle(p);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;        
        battle.gameObject.SetActive(true);
        battle.StartTrainerBattle(trainer.Party);
    }

    void EndBattle(bool playerWin)
    {
        if(playerWin)
        {

        }
        else
        {

        }
        battle.gameObject.SetActive(false);
        state = GameState.Overworld;
    }

    void ExecuteMenuSelection(int selection)
    {
        switch(selection)
        {
            case 0: //Pokedex
                MenuPokedex();
                break;
            case 1: //Pokemon
                MenuPokemon();
                break;
            case 2: //Bag
                MenuBag();
                break;
            case 3: //Trainer
                MenuTrainer();               
                break;
            case 4: //Map 
                MenuMap();
                break;
            case 5: //Save
                MenuSave();
                break;
            case 6: //Options   
                MenuOptions();             
                break;
            case 7:  //Exit
                MenuExit();
                break;            
            default:
                Debug.Log($"Invalid Menu Selection - {selection}");
                break;
        }
    }

    void MenuPokedex()
    {

    }

    void MenuPokemon()
    {
        state = GameState.Pokemon;

        Action onClose = () =>
        {
            pokemonScreen.gameObject.SetActive(false);
            state = GameState.Menu;
        };

        pokemonScreen.OpenScreen(null, onClose);
    }

    void MenuBag()
    {
        state = GameState.Bag;
            
        Action<ItemBase, Pokemon> onClose = (ItemBase itemUsed, Pokemon p) =>
        {
            inventoryUI.gameObject.SetActive(false);
            menuController.OpenMenu();
            state = GameState.Menu;
        };

        inventoryUI.OpenInventory(false, onClose);
        menuController.CloseMenu();
    }

    void MenuTrainer()
    {

    }

    void MenuMap()
    {

    }

    void MenuSave()
    {
        //Confirm if the player wants to save or not
        SaveManager.Instance.SaveGame();
        //Output text saying that the game was saved
    }

    void MenuOptions()
    {

    }

    void MenuExit()
    {
        menuController.CloseMenu();
        state = GameState.Overworld;
    }
}
