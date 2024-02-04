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
                Debug.Log("Max Heal");
                target.TakeDamage(-1*target.Stats.HP);
                hasEffect = true;
            }
            else if(healingAmount > 0)
            {
                Debug.Log($"Healing {healingAmount} HP");
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
                    Debug.Log("Max Elixir used");
                    target.Elixir(-1);
                    hasEffect = true;
                }
                else if(ppAmount > 0)
                {
                    Debug.Log($"Elixir used to heal {ppAmount} pp");
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
                    Debug.Log("Max Ether used");
                    target.Ether(-1, moveSelection);
                    hasEffect = true;
                }
                else if(ppAmount > 0)
                {
                    Debug.Log($"Ether used to heal {ppAmount} pp");
                    target.Ether(ppAmount, moveSelection);
                    hasEffect = true;
                }
            }
        }

        if(healAllStatus)
        {
            if(target.Status != NonVolatileStatus.None)
            {
                Debug.Log("Healing NVStatus");
                target.ClearNVStatus();
                hasEffect = true;
            }
            if(target.ClearVolatileStatuses())
            {
                Debug.Log("Cleared Volatile Statuses");
                hasEffect = true;
            }
        }
        else if(healedStatus != NonVolatileStatus.None && target.Status == healedStatus)
        {
            Debug.Log($"Healing Status: {healedStatus}");
            target.ClearNVStatus();
            hasEffect = true;
        }

        if(revive && target.Fainted)
        {
            Debug.Log("Reviving");
            target.TakeDamage(-1 * target.Stats.HP / 2);
            hasEffect = true;
        }

        if(maxRevive && target.Fainted)
        {
            Debug.Log("Max Reviving");
            target.TakeDamage(-1 * target.Stats.HP);
            hasEffect = true;
        }

        if(maxReviveAll)
        {
            Debug.Log("Max Reviving All");
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
        
        Debug.Log($"Has Effect = {hasEffect}");

        return hasEffect;
    }
}
