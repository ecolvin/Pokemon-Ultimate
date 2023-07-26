using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] HPBar hpBar;
    [SerializeField] TextMeshProUGUI pokemonName;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI genderField;
    [SerializeField] TextMeshProUGUI hpNums;

    //TODO: Replace with parameters
    [SerializeField] PokemonSpecies species;
    
    Pokemon pokemon;

    public void GenerateBar(Pokemon pokemon)
    {
        Debug.Log($"Generating HUD for {pokemon.Nickname}");
        this.pokemon = pokemon;

        pokemonName.text = pokemon.Species.SpeciesName;
        level.text = "Lv." + pokemon.Level;
        if(pokemon.Gender == PokemonGender.Male)
        {
            genderField.text = "♂";
            genderField.color = Color.blue;
        }
        else if(pokemon.Gender == PokemonGender.Female)
        {
            genderField.text = "♀";
            genderField.color = Color.red;
        }
        else
        {
            genderField.text = "";
        }     

        if(hpNums != null)
        {
            hpNums.text = pokemon.CurHP + "/" + pokemon.Stats.HP;
        }
        hpBar.SetHP((float)pokemon.CurHP/(float)pokemon.Stats.HP);
    }

    public IEnumerator UpdateHP()
    {
        int newHP = pokemon.CurHP;
        int maxHP = pokemon.Stats.HP;

        if(maxHP <= 0)
        {
            maxHP = 1;
        }
        if(newHP > maxHP)
        {
            newHP = maxHP;
        }
        if(newHP < 0)
        {
            newHP = 0;
        }

        yield return hpBar.SetHPSmoothly((float) newHP/ (float)maxHP);
        Debug.Log($"HP Nums: {hpNums != null}; New HP: {newHP}; Max HP: {maxHP}");
        if(hpNums != null)
        {
            hpNums.text = newHP + "/" + maxHP;
        }
    }
}
