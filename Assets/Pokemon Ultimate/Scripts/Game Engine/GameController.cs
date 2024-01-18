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

    void Start() 
    {
        player.OnEncounter += StartBattle;    
        battle.OnBattleOver += EndBattle;

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
