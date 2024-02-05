using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public enum InvUIState {Default, Healing, Useable, Learnable}

public class InvParty : MonoBehaviour
{
    [SerializeField] float hopHeight = 10f;
    [SerializeField] float hopSpeed = 5f;

    List<PokemonInvUI> pokemon;
    List<Pokemon> party;

    InvUIState state = InvUIState.Default;

    void Awake()
    {
        pokemon = GetComponentsInChildren<PokemonInvUI>().ToList();
        party = Party.GetParty().Pokemon;
        SetParty();
    }   

    public void SetParty()
    {
        for(int i = 0; i < pokemon.Count; i++)
        {
            if(i < party.Count)
            {
                pokemon[i].Set(party[i]);

            }
            else
            {
                pokemon[i].Set(null);
            }
        }
    }

    public void SetState(InvUIState state)
    {
        foreach(PokemonInvUI p in pokemon)
        {
            p.SetState(state);
        }
    }

    public void UpdateUseable(ItemBase item)
    { 
        foreach(PokemonInvUI p in pokemon)
        {
            //figure out if the item is useable

            p.UseableIcon.color = Color.gray;
            p.UseableText.text = "Cannot Use";

            //useableIcon.color = Color.green;
            //useableText.text = "Can Use";
        }
    }

    public void UpdateLearnable(TM tm)
    {
        foreach(PokemonInvUI p in pokemon)
        {
            //figure out if the tm is learnable

            p.UseableIcon.color = Color.red;
            p.UseableText.text = "Cannot Learn";

            //useableIcon.color = Color.green;
            //useableText.text = "Can Learn";
        }
    }

    public IEnumerator UpdateSelection(int selection)
    {
        PokemonInvUI selectedPokemon = pokemon[selection];
        if(selection >= pokemon.Count)
        {
            yield break;
        }
        if(selectedPokemon.IsMoving)
        {
            yield break;
        }
        selectedPokemon.IsMoving = true;

        Image sprite = selectedPokemon.Sprite;
        float initY = sprite.transform.localPosition.y;
        float targetY = initY + hopHeight;
        float curY = initY;
        float diff = Mathf.Abs(targetY - curY);

        while(curY < targetY)
        {
            curY += diff * hopSpeed * Time.deltaTime;
            if(curY > targetY)
            {
                curY = targetY;
            }
            sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, curY);
            yield return null;
        }

        targetY = initY;

        while(curY > targetY)
        {
            curY -= diff * hopSpeed * Time.deltaTime;
            if(curY < targetY)
            {
                curY = initY;
            }
            sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, curY);
            yield return null;
        }

        selectedPokemon.IsMoving = false;
    }
}

