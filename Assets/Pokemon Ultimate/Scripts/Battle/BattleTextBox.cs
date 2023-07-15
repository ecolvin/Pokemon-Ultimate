using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleTextBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleText;
    
    public void wildPokemonIntro(string pokemonName)
    {
        battleText.text = "A wild " + pokemonName + " has appeared!";
    }
}
