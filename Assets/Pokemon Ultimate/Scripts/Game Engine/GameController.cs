using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {Overworld, Battle, Dialog}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Battle battle;

    GameState state;
    Route curRoute;

    public static GameController Instance {get; private set;}

    void Start() 
    {
        Instance = this;
        player.OnEncounter += StartBattle;    
        battle.OnBattleOver += EndBattle;

        player.OnTrainerBattle += (Collider trainerCollider) =>
        {
            TrainerController trainer = trainerCollider.GetComponent<TrainerController>();
            if(trainer != null)
            {

            }
        };

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
    }

    void Update() 
    {
        if(state == GameState.Overworld)
        {
            player.HandleUpdate();
        }    
        else if(state == GameState.Battle)
        {
            battle.HandleUpdate();
        }
        else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    void StartBattle(int shinyRolls, int haRolls, int numPerfect)
    {
        state = GameState.Battle;        
        curRoute = FindObjectOfType<Route>();
        battle.gameObject.SetActive(true);
        battle.StartBattle(curRoute.GetPokemon(Habitat.Grass, TimePeriod.Day, shinyRolls, haRolls, numPerfect));
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
}
