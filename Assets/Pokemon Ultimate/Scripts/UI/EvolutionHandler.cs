using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionHandler : MonoBehaviour
{
    [SerializeField] GameObject evolutionScreen;
    [SerializeField] Image pokemonSprite;

    public static EvolutionHandler Instance {get; private set;}

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evo)
    {
        Sprite oldSprite = pokemon.Sprite;
        string oldNickname = pokemon.Nickname;
        evolutionScreen.SetActive(true);
        pokemonSprite.sprite = oldSprite;
        yield return DialogManager.Instance.ShowDialog($"{oldNickname} is evolving!");
        
        pokemon.Evolve(evo);        
        Sprite newSprite = pokemon.Sprite;

        pokemonSprite.sprite = pokemon.Sprite;
        yield return DialogManager.Instance.ShowDialog($"{oldNickname} evolved into {pokemon.Species.SpeciesName}!");
        evolutionScreen.SetActive(false);
    }
}
