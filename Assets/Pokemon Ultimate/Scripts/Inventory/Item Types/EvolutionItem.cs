using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Evolution Item")]
public class EvolutionItem : ItemBase
{
    [SerializeField] List<PokemonSpecies> usablePokemon;

    public override bool CanUseInBattle => false;

    public override bool Use(Pokemon target)
    {
        if(target.CheckForItemEvo(this) == null)
        {
            return false;
        }
        return Useable(target);
    }

    public override bool Useable(Pokemon target)
    {
        if(target == null || usablePokemon.Count == 0)
        {
            return false;
        }
        return usablePokemon.Contains(target.Species);
    }
}
