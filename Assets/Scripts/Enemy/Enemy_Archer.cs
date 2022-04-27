using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy_Archer : Enemy_Generic
{
    [Header("Attack Telegraph Settings")]
    public float telegraphTime = 0.5f;
    //make enemy glow red, and make point light intensity go to 2.5
    public float SaturationValue = 0;

    // telegraph time / eachStepTime = number of steps
    public float eachStepTime = 0.05f;
    //glow interval should be 70 / number of steps
    public float glowInterval = 0.1f;

    [Header("Archer Settings")]
    public float shootInterval;
    public float shootTimer;
    public float arrowDamage;
    public float arrowCritChance;
    public float arrowCritDamage;

    //References
    public GameObject arrowPrefab;



    private void doDamage()
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

        arrowDamage = damage;
        arrowCritChance = (float)critChance;
        arrowCritDamage = (float)critDamage;
    }


    private void bowAttack()
    {
        doDamage(); //update the damage and crit and damage with buffs before we send off arrow, arrow will calculate crit when hit target
        
        print("Shoot!");
        if (topPriorityTarget != null)
        {
            Vector3 direction = (topPriorityTarget.transform.position - this.transform.position).normalized;
            Vector2 dir = new Vector2(direction.x, direction.y);

            Debug.DrawRay(this.transform.position, direction, Color.green, 2f);
            StartCoroutine(doAttack(dir));
        }
    }
    private void launchArrow(Vector2 dir)
    {
        GameObject arrowInstance = GameObject.Instantiate(arrowPrefab);
        arrowInstance.GetComponent<ArrowDetails>().playerArrow = false;
        arrowInstance.GetComponent<ArrowDetails>().setDetails(arrowDamage, arrowCritChance, arrowCritDamage);


        //position arrow and send it off
        arrowInstance.transform.position = this.transform.position;
        Vector3 angle = new Vector3(0f, 0f, Mathf.Atan2(dir.y, dir.x)*180/Mathf.PI);
        arrowInstance.transform.eulerAngles = angle;
        arrowInstance.GetComponent<Rigidbody2D>().velocity = dir * 10f;
         
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //Also decreament timer for how often to shoot
        if (shootTimer < shootInterval)
        {
            shootTimer += 1 * Time.deltaTime;
        }
        else
        {
            bowAttack();
            shootTimer = 0;
        }
    }

    

    private IEnumerator doAttack(Vector2 dir)
    {
        attacking = true;
        //set telegraph
        StartCoroutine(setEnemyTelegraph());
        yield return new WaitForSeconds(telegraphTime);
        enemyAnimations.bowAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        launchArrow(dir);
        yield return new WaitForSeconds(0.7f);
        SaturationValue = 0f;
        Color color = new Color(0.764151f, 0.5635262f, 0.3640531f);
        sprite.color = color;
        yield return new WaitForSeconds(4f);
        attacking = false;
    }

    
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
