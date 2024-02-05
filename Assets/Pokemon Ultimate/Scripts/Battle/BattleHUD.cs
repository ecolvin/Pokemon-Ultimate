using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;
    [SerializeField] TextMeshProUGUI pokemonName;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI genderField;
    [SerializeField] TextMeshProUGUI hpNums;
    [SerializeField] StatusIcon statusIcon;

    //TODO: Replace with parameters
    [SerializeField] PokemonSpecies species;
    
    Pokemon pokemon;

    public void GenerateBar(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        if(pokemon == null)
        {
            return;
        }
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
        UpdateStatus();

        SetExp(pokemon.GetExpPercent());
    }

    void LevelUpLevelText()
    {
        int lvl = 0;
        if(!Int32.TryParse(level.text.Substring(3), out lvl))
        {
            Debug.Log($"\"{level.text.Substring(3)}\" cannot be parsed.");
        }
        else
        {
            level.text = "Lv." + ++lvl;
        }
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
        
        if(hpNums != null)
        {
            hpNums.text = newHP + "/" + maxHP;
        }
    }

    public void SetExp(float expPercentage)
    {
        if(expBar == null)
        {
            return;
        }

        float newExp = expPercentage;
        while(newExp > 1)
        {
            newExp -= 1;
        }
        if(newExp < 0)
        {
            newExp = 0;
        }

        expBar.transform.localScale = new Vector3(newExp, 1f, 1f);
    }

    public IEnumerator LevelUp()
    {
        if(expBar == null)
        {
            yield break;
        }

        float curExp = expBar.transform.localScale.x;
        float newExp = 1;
        
        float diff = newExp - curExp;
        
        while(newExp - curExp > Mathf.Epsilon)
        {
            curExp += diff * Time.deltaTime;
            expBar.transform.localScale = new Vector3(curExp, 1f, 1f);

            yield return null;
        }

        expBar.transform.localScale = new Vector3(0f, 1f, 1f);
        LevelUpLevelText();
    }

    public IEnumerator UpdateXP(float xpPercentage)
    {
        if(expBar == null)
        {
            yield break;
        }

        float curExp = expBar.transform.localScale.x;
        float newExp = xpPercentage;
        if(newExp > 1)
        {
            yield return LevelUp();
            newExp -= 1f;
        }
        if(newExp < 0)
        {
            newExp = 0;
        }
        
        float diff = newExp - curExp;
        
        while(newExp - curExp > Mathf.Epsilon)
        {
            curExp += diff * Time.deltaTime;
            expBar.transform.localScale = new Vector3(curExp, 1f, 1f);

            yield return null;
        }

        expBar.transform.localScale = new Vector3(newExp, 1f, 1f);
    }

    public void UpdateStatus()
    {
        if(statusIcon != null)
        {
            statusIcon.UpdateBattleIcon(pokemon.Status);
        }
    }
}
