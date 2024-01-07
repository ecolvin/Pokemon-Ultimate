using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMember : MonoBehaviour
{
    [SerializeField] Image spriteImage;
    [SerializeField] BattleHUD hud;
    [SerializeField] Color defaultColor;
    [SerializeField] Color selectionColor;

    Pokemon poke;
    public Pokemon Poke{get{return poke;}}

    public void Set(Pokemon pokemon)
    {
        poke = pokemon;
        hud.gameObject.SetActive(true);
        spriteImage.gameObject.SetActive(true);
        hud.GenerateBar(pokemon);
        Debug.Log("4");
        spriteImage.sprite = pokemon.Sprite;
        Debug.Log("5");
    }

    public void Disable()
    {
        hud.gameObject.SetActive(false);
        spriteImage.gameObject.SetActive(false);
    }

    public void Default()
    {
        hud.GetComponent<Image>().color = defaultColor;
    }

    public void Select()
    {
        hud.GetComponent<Image>().color = selectionColor;
    }
}
