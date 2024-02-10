using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PokemonSummary : MonoBehaviour
{
    [SerializeField] Image sprite;
    [SerializeField] TextMeshProUGUI speciesName;
    [SerializeField] TextMeshProUGUI nickname;
    [SerializeField] Image type1Icon;
    [SerializeField] Image type2Icon;
    [SerializeField] Image genderIcon;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] StatBox hpBox;
    [SerializeField] StatBox atkBox;
    [SerializeField] StatBox defBox;
    [SerializeField] StatBox spaBox;
    [SerializeField] StatBox spdBox;
    [SerializeField] StatBox speBox;
    [SerializeField] TextMeshProUGUI abilityText;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] Image itemSprite;

    //Store these globally in the future
    [SerializeField] Sprite maleGenderIcon;
    [SerializeField] Sprite femaleGenderIcon;

    Pokemon pokemon;
    public Pokemon Pokemon{get => pokemon;}

    public void Set(Pokemon p)
    {
        if(p == null)
        {
            sprite.color = Color.clear;
            speciesName.text = "";
            nickname.text = "";
            type1Icon.color = Color.clear;
            type2Icon.color = Color.clear;
            genderIcon.color = Color.clear;
            levelText.text = "";
            
            hpBox.SetHP(0, 0);
            atkBox.SetStat(0);
            defBox.SetStat(0);
            spaBox.SetStat(0);
            spdBox.SetStat(0);
            speBox.SetStat(0);

            hpBox.SetNature(0);
            atkBox.SetNature(0);
            defBox.SetNature(0);
            spaBox.SetNature(0);
            spdBox.SetNature(0);
            speBox.SetNature(0);

            abilityText.text = "--";
            itemText.text = "--";
            itemSprite.color = Color.clear;
            return;
        }

        sprite.sprite = p.Sprite;
        speciesName.text = p.Species.SpeciesName;
        nickname.text = p.Nickname;
        type1Icon.color = Color.white;
        type1Icon.sprite = GlobalSpriteDictionary.Instance.TypeIcons[p.Type1];
        if(p.Type2 != PokemonType.None)
        {
            type2Icon.color = Color.white;
            type2Icon.sprite = GlobalSpriteDictionary.Instance.TypeIcons[p.Type2];
        }
        else
        {
            type2Icon.color = Color.clear;
        }

        switch(p.Gender)
        {
            case PokemonGender.Male:
                genderIcon.color = Color.white;
                genderIcon.sprite = maleGenderIcon;
                break;
            case PokemonGender.Female:
                genderIcon.color = Color.white;
                genderIcon.sprite = femaleGenderIcon;
                break;
            default:
                genderIcon.color = Color.clear;
                break;
        }

        levelText.text = $"Lv. {p.Level}";

        hpBox.SetHP(p.CurHP, p.Stats.HP);
        atkBox.SetStat(p.Stats.Atk);
        defBox.SetStat(p.Stats.Def);
        spaBox.SetStat(p.Stats.SpA);
        spdBox.SetStat(p.Stats.SpD);
        speBox.SetStat(p.Stats.Spe);

        hpBox.SetNature(0);
        atkBox.SetNature(GetNatureBonus(p.Nature, 1));
        defBox.SetNature(GetNatureBonus(p.Nature, 2));
        spaBox.SetNature(GetNatureBonus(p.Nature, 3));
        spdBox.SetNature(GetNatureBonus(p.Nature, 4));
        speBox.SetNature(GetNatureBonus(p.Nature, 5));

        abilityText.text = p.Ability;

        if(p.HeldItem != null)
        {
            itemText.text = p.HeldItem.ItemName;
            itemSprite.sprite = p.HeldItem.Icon;
            itemSprite.color = Color.white;
        }
        else
        {
            itemText.text = "--";
            itemSprite.color = Color.clear;
        }
    }

    int GetNatureBonus(PokemonNature nature, int stat)
    {
        int dec = (int) nature % 10; //Ones place of enum
        int inc = (int) nature / 10; //Tens place of enum

        if(inc == dec)
        {
            return 0;
        }

        if(stat == inc)
        {
            return 1;
        }
        else if(stat == dec)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
