using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/TM")]
public class TM : ItemBase
{
    [SerializeField] int number;
    [SerializeField] PokemonMove move;
}
