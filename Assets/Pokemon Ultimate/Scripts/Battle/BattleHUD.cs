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

    [SerializeField] [Range(1, 100)] int damageSpeed = 10;

    //TODO: Replace with parameters
    [SerializeField] PokemonSpecies species;
    
    Pokemon pokemon;

    public void GenerateBar(Pokemon pokemon)
    {
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
        UpdateHP();
    }

    public IEnumerator UpdateHP()
    {
        int newHP = pokemon.CurHP;
        int maxHP = pokemon.Stats[0];

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
        if(hpNums != null)
        {
            hpNums.text = newHP + "/" + maxHP;
        }
    }
}
