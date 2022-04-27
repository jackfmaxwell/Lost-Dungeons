using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


//This class inherits from the generic enemy and implements a shield enemy behaviour over top
public class Enemy_Shield : Enemy_Generic
{

    private bool reached = false; //reached protect destination

    //skill details
        //object
    [SerializeField]
    private GameObject shieldRing;
        //cooldowncurrent
    [SerializeField]
    private float shieldTimer;
        //cooldown total
    [SerializeField]
    private float shieldInerval;

    //skill buff details
    [SerializeField]
    private BuffDebuffObjectScript buff;
    [SerializeField]
    private float buffDuration;

    //set desired proximity
    protected override void Start()
    {
        float temp = desiredPromixtyToTarget;
        base.Start();
        desiredPromixtyToTarget = temp;
    }

    //do normal fixed update and also do the shield skill cooldowns and activation
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (shieldTimer < shieldInerval)
        {
            shieldTimer += 1 * Time.deltaTime;
        }
        else
        {
            if (reached)
            {
                spawnShield();
                shieldTimer = 0;
            }


        }
    }

    //This method updates the top priority target, we dont want to set it to a target from our list we want the pri target
    //to be a nearby ally. 
    protected override void update_topPriorityTarget()
    {
        protectNearbyAllies();
        if (topPriorityTarget == null)
        {
            desiredMovement = new Vector2(0f, 0f);

        }
    }
    //Find the most damaged enemy and go towards them
    private void protectNearbyAllies()
    {
        float downBadHealthPercent = 100;
        Enemy_Generic downBadEnemy = null;
        foreach (Transform ally in nearbyAllies)
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
            if (nearbyAllies.Contains(downBadEnemy.transform))
            {
                topPriorityTarget = downBadEnemy.transform;
            }

        }
    }

    //if we arrive at our destination
    public override void reachedDestination()
    {
        reached = true;
    }

    //Spawn shield physical object 
    private void spawnShield()
    {
        //instantiate
        GameObject ring = GameObject.Instantiate(shieldRing);
        ring.transform.position = this.transform.position;
        
        //set details
        ring.GetComponent<RingDetails>().playerSkill = false;
        ring.GetComponent<RingDetails>().duration = buffDuration;

        //set buff details
        Player_Buffs.ActiveBuffDebuff buffdebuff = new Player_Buffs.ActiveBuffDebuff(buff, buffDuration);
        ring.GetComponent<RingDetails>().buff = buffdebuff;
    }
}



