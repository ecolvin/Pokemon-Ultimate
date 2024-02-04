using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class InvParty : MonoBehaviour
{
    [SerializeField] float hopHeight = 10f;
    [SerializeField] float hopSpeed = 5f;

    List<PokemonInvUI> pokemon;
    List<Pokemon> party;

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
                pokemon[i].Sprite.gameObject.SetActive(true);
                pokemon[i].Sprite.sprite = party[i].Sprite;
            }
            else
            {
                pokemon[i].Sprite.gameObject.SetActive(false);
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
