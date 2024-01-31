using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {Overworld, Battle, Dialog, Menu}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Battle battle;

    GameState state;
    //Route curRoute;

    MenuController menuController;

    public SceneDetails CurScene {get; private set;}
    public SceneDetails PrevScene {get; private set;}

    public static GameController Instance {get; private set;}

    void Start() 
    {
        Instance = this;
        menuController = GetComponent<MenuController>();

        player.OnEncounter += StartBattle;    
        battle.OnBattleOver += EndBattle;
        menuController.OnSelection += ExecuteMenuSelection;

        DialogManager.Instance.OnShowDialog += () => 
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () => 
        {
            if(state == GameState.Dialog)
            {
                state = GameState.Overworld;
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
            case 3: //Save
                MenuSave();
                break;
            case 4: //Trainer 
                MenuTrainer();               
                break;
            case 5: //Options   
                MenuOptions();             
                break;
            case 6:  //Exit
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

    }

    void MenuBag()
    {

    }

    void MenuSave()
    {
        //Confirm if the player wants to save or not
        SaveManager.Instance.SaveGame();
        //Output text saying that the game was saved
    }

    void MenuTrainer()
    {

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
