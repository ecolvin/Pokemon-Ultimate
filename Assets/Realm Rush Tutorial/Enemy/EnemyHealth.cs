using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 5;

    [Tooltip("Adds specified amount to maxHP when enemy dies")]
    [SerializeField] int HPIncrease = 1;
    
    [SerializeField] [Range(1,5)] int damage = 1;

    int currentHP = 0;

    Enemy enemy;
    void OnEnable()
    {
        currentHP = maxHP;
    }

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update() 
    {
        
    }

    void OnParticleCollision(GameObject other)
    {
        ProcessHit();
    }

    void ProcessHit()
    {
        currentHP -= damage;
        if(currentHP <= 0)
        {
            gameObject.SetActive(false);
            enemy.RewardGold();
            maxHP += HPIncrease;
        }    
    }
}
