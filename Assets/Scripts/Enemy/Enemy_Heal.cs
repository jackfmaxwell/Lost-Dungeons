using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy_Heal : Enemy_Generic
{
    public float healtimer=0;
    public float healInterval=10;
    public float healAmount=10;


    //not all enemys can do damage
    public override void reachedDestination()
    {
        if (distanceToPriorityTarget() +2f < desiredPromixtyToTarget)
        {
            //need to get away
            backup();
            followEnabled = false;
        }
    }

    protected override void Start()
    {
        float temp = desiredPromixtyToTarget;
        base.Start();
        desiredPromixtyToTarget = temp;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //Also decreament timer for how often to heal
        if (healtimer < healInterval)
        {
            healtimer += 1 * Time.deltaTime;
        }
        else
        {
            healNearbyAllies();
            healtimer = 0;
        }
    }

    private void healNearbyAllies()
    {
        float downBadHealthPercent = 100;
        Enemy_Generic downBadEnemy=null;
        foreach(Transform ally in nearbyAllies)
        {
            float health = ally.GetComponent<Enemy_Generic>().currentHealth;
            if (downBadHealthPercent > (health / ally.GetComponent<Enemy_Generic>().enemyDetails.health) * 100 || downBadEnemy == null)
            {
                downBadEnemy = ally.GetComponent<Enemy_Generic>();
                downBadHealthPercent = (health / ally.GetComponent<Enemy_Generic>().enemyDetails.health) * 100;
            }
        }

        //Found the most down bad enemy
        if (downBadEnemy != null)
        {
            downBadEnemy.heal(healAmount);
        }
    }
}
