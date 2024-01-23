using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyHUD : MonoBehaviour
{
    [SerializeField] Image [] pokeSlots = new Image[6];

    public void Set(Party party)
    {
        int i = 0;
        foreach(Pokemon p in party.PartyPokemon)
        {
            pokeSlots[i].gameObject.SetActive(true);

            Debug.Log($"Pokemon = {p}");

            if(p.Fainted)
            {
                pokeSlots[i].color = new Color(.25f,.25f,.25f,1f);
            }
            else if(p.Status != NonVolatileStatus.None)
            {
                pokeSlots[i].color = Color.yellow;
            }

            i++;
        }
        while(i < 6)
        {
            pokeSlots[i].gameObject.SetActive(false);

            i++;
        }
    }
}
