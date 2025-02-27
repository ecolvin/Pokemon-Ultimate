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
        spriteImage.sprite = pokemon.Sprite;
        hud.gameObject.SetActive(true);
        spriteImage.gameObject.SetActive(true);
        poke.OnDataChange += Update;
    }

    void Update()
    {
        hud.GenerateBar(poke);
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
