using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP Recovery")]
    [SerializeField] int healingAmount;
    [SerializeField] bool maxHeal;

    [Header("PP Recovery")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;
    [SerializeField] bool restoreAllMoves;

    [Header("Status Recovery")]
    [SerializeField] NonVolatileStatus healedStatus;
    [SerializeField] bool healAllStatus;

    [Header("Happiness")]
    [SerializeField] bool lowersHappiness;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;
    [SerializeField] bool maxReviveAll;


    public override bool Use(Pokemon target)
    {
        bool hasEffect = false;
        if(target.CurHP < target.Stats.HP && !target.Fainted)
        {
            if(maxHeal)
            {
                target.TakeDamage(-1*target.Stats.HP);
                hasEffect = true;
            }
            else if(healingAmount > 0)
            {
                target.TakeDamage(-1*healingAmount);
                hasEffect = true;
            }
        }

        if(!target.PPFull())
        {        
            if(restoreAllMoves)
            {
                if(restoreMaxPP)
                {
                    target.Elixir(-1);
                    hasEffect = true;
                }
                else if(ppAmount > 0)
                {
                    target.Elixir(ppAmount);
                    hasEffect = true;
                }
            }
            else
            {
                //IMPLEMENT UI TO CHOOSE A MOVE
                int moveSelection = 0;
                if(restoreMaxPP)
                {
                    target.Ether(-1, moveSelection);
                    hasEffect = true;
                }
                else if(ppAmount > 0)
                {
                    target.Ether(ppAmount, moveSelection);
                    hasEffect = true;
                }
            }
        }

        if(healAllStatus)
        {
            if(target.Status != NonVolatileStatus.None)
            {
                target.ClearNVStatus();
                hasEffect = true;
            }
            if(target.ClearVolatileStatuses())
            {
                hasEffect = true;
            }
        }
        else if(healedStatus != NonVolatileStatus.None && target.Status == healedStatus)
        {
            target.ClearNVStatus();
            hasEffect = true;
        }

        if(revive && target.Fainted)
        {
            target.TakeDamage(-1 * target.Stats.HP / 2);
            hasEffect = true;
        }

        if(maxRevive && target.Fainted)
        {
            target.TakeDamage(-1 * target.Stats.HP);
            hasEffect = true;
        }

        if(maxReviveAll)
        {
            List<Pokemon> party = Party.GetParty().Pokemon;
            foreach(Pokemon p in party)
            {
                if(p.Fainted)
                {
                    p.TakeDamage(-1 * p.Stats.HP);
                    hasEffect = true;
                }
            }
        }
        
        if(lowersHappiness && hasEffect)
        {
            //Lower the pokemon's happiness
        }

        return hasEffect;
    }

    // public override bool UsableOn(Pokemon target)
    // {
    //     //If target is fainted, only revives are usable. Everything else returns false.
    //     if(target.Fainted)
    //     {
    //         if(revive || maxRevive || maxReviveAll)
    //         {
    //             return true;
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }

    //     //If item heals hp and pokemon is not at full HP return true;
    //     if(target.CurHP < target.Stats.HP &&  (maxHeal || healingAmount > 0))
    //     {
    //         return true;
    //     }

    //     //If item heals pp and target has a move that can be restored
    //     if(!target.PPFull() && (restoreAllMoves || restoreMaxPP || ppAmount > 0))
    //     {        
    //         return true;
    //     }

    //     //If item heals status conditions and target has a status condition that is healed by the item
    //     if(target.Status != NonVolatileStatus.None && (healAllStatus || target.Status == healedStatus))
    //     {
    //         return true;
    //     }

    //     return false;
    // }
}
