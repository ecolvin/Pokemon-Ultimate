using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Pokeball")]
public class Pokeball : ItemBase
{
    [SerializeField] float baseCatchRate = 1f;
    [SerializeField] bool isHeavyBall = false;
    public bool IsHeavyBall {get => isHeavyBall;}

    public void OnCatch(Pokemon caughtPokemon)
    {
        //if(healBall)
        //{
        //  //Heal Pokemon
        //}
        //if(FriendBall)
        //{
        //  //Increase Friendship
        //}
    }

    public float GetCatchRate(Pokemon target)
    {
        return baseCatchRate;
    }
}
