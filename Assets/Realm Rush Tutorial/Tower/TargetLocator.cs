using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLocator : MonoBehaviour
{
    [SerializeField] Transform weapon;
    [SerializeField] ParticleSystem projectileSystem;
    [SerializeField] float range = 1.5f;

    Transform target = null;

    void Update()
    {
        FindClosestTarget();
        if(target != null)
        {
            AimWeapon();
        }
    }

    void FindClosestTarget()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Transform closestTarget = null;
        float maxDistance = Mathf.Infinity;

        foreach(Enemy enemy in enemies)
        {
            float targetDistance = Vector3.Distance(transform.position, enemy.transform.position);
            
            if(targetDistance < maxDistance)
            {
                closestTarget = enemy.transform;
                maxDistance = targetDistance;
            }
        }

        target = closestTarget;
    }

    void AimWeapon()
    {
        float targetDistance = Vector3.Distance(transform.position, target.position);

        if(targetDistance <= range)
        {
            Attack(true);
        }
        else
        {
            Attack(false);
        }

        weapon.LookAt(target);
    }

    void Attack(bool isActive)
    {
        var emissionModule = projectileSystem.emission;
        emissionModule.enabled = isActive;
    }
}
