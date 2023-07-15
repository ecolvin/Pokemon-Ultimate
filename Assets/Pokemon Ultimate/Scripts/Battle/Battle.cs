using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleTextBox dialog;
    
    //TODO: Replace with parameters
    [SerializeField] Image background;
    [SerializeField] Image enemySprite;
    [SerializeField] Image playerSprite;

    //TODO: Replace with parameters
    [SerializeField] PokemonSpecies enemySpecies;
    [SerializeField] PokemonSpecies playerSpecies;

    void Start() 
    {        
        startBattle(new Pokemon(playerSpecies, 5, true, false), new Pokemon(enemySpecies, 5, true, false));
    }

    public void startBattle(Pokemon playerPokemon, Pokemon enemyPokemon)
    {
        dialog.wildPokemonIntro(enemyPokemon.Species.SpeciesName);
        playerSprite.sprite = playerPokemon.Sprite;
        enemySprite.sprite = enemyPokemon.Sprite;
        enemyHUD.generateBar(enemyPokemon);
        playerHUD.generateBar(playerPokemon);
    }
}
