using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy_Assault : Enemy_Generic
{
    public BoxCollider2D attackBox;
    public LayerMask playerMask;
    
    public bool attackCoolingDown = false;
    public float attackCooldownTime = 1.5f;

    //have to reimplement start and call it from this class
    protected override void Start()
    {
        base.Start();
        //slightly randomize the attack cooldown
        attackCooldownTime = Random.Range(0.25f, 2f);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void swordAttackCheck()
    {
        //Check the intersects of the attack hit box and doDamage() to the targets
        Collider2D[] results = new Collider2D[4];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerMask);
        attackBox.OverlapCollider(filter, results);
        foreach (Collider2D col in results)
        {
            if (col != null)
            {
                if (col.gameObject.tag == "Player")
                {
                    //Go ahead and do damage
                    doDamage(col.gameObject);

                }
            }

        }
    }
    private void doDamage(GameObject player) //MeleeType is true if melee, false if bow
    {
        float totalDamage = 0;
        //calculate buffs
        float damage = 0;
        double critChance = 0, critDamage = 0;
        if (enemyDetails is EnemyObjectScript)
        {
            //Get initial details
            damage = enemyDetails.damage;

            critChance = enemyDetails.critChance;
            critDamage = enemyDetails.critDamage;

            //loop buffs 
            
            foreach (ActiveBuffDebuff item in activeBuffsDebuffs)
            {
                BuffDebuffObjectScript buff = item.buffDebuff;
                if (buff is BuffDebuffObjectScript) //check for sriptableobjects to work
                {
                    //Get the buff info and apply it
                    if (buff.increaseorDecrease)
                    {
                        //this buff increases the stat
                        if (buff.statToModify == BuffDebuffObjectScript.stats.critChance) //CC
                        {
                            //make sure critchance doesnt go more than 100%
                            if ((critChance * (1f + (buff.amount) / 100f)) > 100f)
                            {
                                critChance = 100f;
                            }
                            else
                            {
                                critChance *= (1f + (buff.amount) / 100f);
                            }

                        }
                        if (buff.statToModify == BuffDebuffObjectScript.stats.critDamage) //CD
                        {
                            critDamage *= (1f + (buff.amount) / 100f);
                        }
                    }
                    else
                    {
                        //Decrease the stat
                        if (buff.statToModify == BuffDebuffObjectScript.stats.critChance)//CC
                        {
                            //Make sure crit chance doesnt go below 0
                            if ((critChance / (1f + (buff.amount / 100f))) > 0)
                            {
                                critChance /= (1f + (buff.amount) / 100f);
                            }
                            else
                            {
                                critChance = 0f;
                            }

                        }
                        if (buff.statToModify == BuffDebuffObjectScript.stats.critDamage)//CD
                        {
                            critDamage /= (1f + (buff.amount) / 100f);
                        }

                    }
                }
            }
            
        }

        //calculate crit
        float rollDice = UnityEngine.Random.value;
        if (rollDice < (float)critChance / 100)
        {
            //ITS A CRIT
            //how much extra damage?
            totalDamage = damage + (damage * ((float)critDamage / 100f)); //turn to percent
            //Null check for player class, then do damage
            if (player.GetComponent<Player_Combat>() != null)
            {
                Player_Combat playerCombat = player.GetComponent<Player_Combat>();
                playerCombat.takeDamage(totalDamage, true, this.gameObject);
            }
            else
            {
                Debug.LogError("The Assualt Enemy could not find the player combat class to apply damage");
            }
           
            
        }
        else
        {
            //NOT A CRIT
            totalDamage = damage;
            //Null check for player class, then do damage
            if (player.GetComponent<Player_Combat>() != null)
            {
                Player_Combat playerCombat = player.GetComponent<Player_Combat>();
                playerCombat.takeDamage(totalDamage, true, this.gameObject);
            }
            else
            {
                Debug.LogError("The Assualt Enemy could not find the player combat class to apply damage");
            }
        }
    }

    //not all enemys can do damage
    public override void reachedDestination()
    {
        if (!attackCoolingDown)
        {
            StartCoroutine(doAttack());
        }
        
    }
  

    public float telegraphTime = 0.5f;

    private IEnumerator doAttack()
    {
        attacking = true;
        desiredMovement = new Vector2(0f, 0f);
        //follow disabled
        followEnabled = false;
        //set telegraph
        StartCoroutine(setEnemyTelegraph());
        //wait
        yield return new WaitForSeconds(telegraphTime);
        //do attack
        enemyAnimations.swordAttackAnimation();
        yield return new WaitForSeconds(0.3f);
        swordAttackCheck();
        yield return new WaitForSeconds(0.1f);
        SaturationValue = 0f;
        Color color = new Color(1f, 0.7028302f, 0.7028302f); //NOTE: NOT WHITE, TINTED RED FOR DISPLAY
        sprite.color = color;
        //Finsh attack
        attacking = false;
        //Start attack cooldown
        attackCoolingDown = true;
        desiredPromixtyToTarget = desiredPromixtyToTarget + 1.5f; //how far the enemy will stay backed away from AI waiting for attack to recharge
        backup(); //do backup
        yield return new WaitForSeconds(attackCooldownTime);
        attackCoolingDown = false;
        desiredPromixtyToTarget = desiredPromixtyToTarget - 1.5f; //set back to normal


    }

    [Header("Attack Telegraph Settings")]
    //make enemy glow red, and make point light intensity go to 2.5
    public float SaturationValue = 0;

    // telegraph time / eachStepTime = number of steps
    public float eachStepTime = 0.05f;
    //glow interval should be 70 / number of steps
    public float glowInterval = 0.1f;
    private IEnumerator setEnemyTelegraph()
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / telegraphTime)
        {
            Color color = Color.HSVToRGB(0f, Mathf.Lerp(0f, 1f, t), 1f);
            sprite.color = color;
            yield return null;
        }

    }

}
