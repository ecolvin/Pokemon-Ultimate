using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int reward = 25;
    [SerializeField] int penalty = 25;

    Bank bank;

    void Start()
    {
        bank = FindObjectOfType<Bank>();        
    }

    public void RewardGold()
    {
        if(bank == null){return;}
        bank.Deposit(reward);
    }

    public void StealGold()
    {
        if(bank == null){return;}
        bank.Withdraw(penalty);
    }
}
