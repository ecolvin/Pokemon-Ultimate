using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public enum InvUIState {Default, Healing, Useable, Learnable, HeldItem, Experience}

public class InvParty : MonoBehaviour
{
    [SerializeField] float hopHeight = 10f;
    [SerializeField] float hopSpeed = 5f;

    List<PokemonInvUI> pokemon;
    List<Pokemon> party;

    InvUIState curState = InvUIState.Default;

    void Awake()
    {
        pokemon = GetComponentsInChildren<PokemonInvUI>().ToList();
        Party p = Party.GetParty();
        party = p.Pokemon;
        p.OnUpdated += SetParty;
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
        if(curState == state)
        {
            return;
        }
        curState = state;
        foreach(PokemonInvUI p in pokemon)
        {
            p.SetState(state);
        }
    }

    public void UpdateUseable(ItemBase item)
    { 
        foreach(PokemonInvUI p in pokemon)
        {
            if(item.Useable(p.Pokemon))
            {
                p.UseableIcon.color = Color.green;
                p.UseableText.text = "Can Use";
            }
            else
            {
                p.UseableIcon.color = Color.gray;
                p.UseableText.text = "Cannot Use";
            }

        }
    }

    public void UpdateLearnable(TM tm)
    {
        foreach(PokemonInvUI p in pokemon)
        {
            if(p.Pokemon == null)
            {
                continue;
            }
            if(p.Pokemon.Moves.FirstOrDefault(m => m.MoveBase == tm.Move) != null)
            {
                p.UseableIcon.color = Color.grey;
                p.UseableText.text = "Learned"; //"Already Learned" is the correct text but it doesn't fit in the bubble currently
            }
            else if(p.Pokemon.Species.TMLearnset.Contains(tm.Move))
            {
                p.UseableIcon.color = Color.green;
                p.UseableText.text = "Can Learn";
            }
            else
            {
                p.UseableIcon.color = Color.red;
                p.UseableText.text = "Cannot Learn";
            }
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

