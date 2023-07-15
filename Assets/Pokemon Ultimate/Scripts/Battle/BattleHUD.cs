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

    public void generateBar(Pokemon pokemon)
    {
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
        updateHP(pokemon.CurHP, pokemon.Stats[0]);
    }

    public void updateHP(int curHP, int maxHP)
    {
        if(maxHP < 0)
        {
            maxHP = 0;
        }
        if(curHP > maxHP)
        {
            curHP = maxHP;
        }
        if(curHP < 0)
        {
            curHP = 0;
        }
        if(hpNums != null)
        {
            hpNums.text = curHP + "/" + maxHP;
        }
        hpBar.setHP((float) curHP/ (float)maxHP);
    }
}
