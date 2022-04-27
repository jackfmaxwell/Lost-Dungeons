using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


//Class to grab details about arrow damage and ect so enemy can read it
public class ArrowDetails : NetworkBehaviour
{
    public Player_Talents playerTalents; //link to the talents so can call the event

    public bool playerArrow = true;
    private void Awake()
    {
        if (playerArrow)
        {
            this.gameObject.tag = "Player_Arrow";
        }
        else
        {
            this.gameObject.tag = "Enemy_Arrow";
        }
    }

    //vars
    private float damage;
    private double critChance, critDamage;
    private bool crit = false;

    //Getters and setters
    public void setDetails(float dmg, double critChance, double critDamage)
    {
        damage = dmg;
        this.critChance = critChance;
        this.critDamage = critDamage;
    }
    public float getDamage()
    {
        float rollDice = UnityEngine.Random.value;
        if (rollDice < (float)critChance / 100)
        {
            //ITS A CRIT
            crit = true;
            return damage + damage * ((float)critDamage / 100f);
        }
        else
        {
            //NO CRIT
            crit = false;
            return damage;
        }

    }
    public bool getCrit()
    {
        return crit;
    }


    //destroy arrow on collision
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Wall" || collision.gameObject.layer == 11) //door
        {

            GameObject.Destroy(this.gameObject);
        }

        if (playerArrow)
        {
            //arrow does the damage to the enemy
            if (collision.gameObject.tag == "Enemy")
            {
                float damage = getDamage();
                bool crit = getCrit();
               
                collision.GetComponent<Enemy_Generic>().takeDamage(damage / 2f, crit, this.transform.position.x - collision.gameObject.transform.position.x);
                if (playerTalents != null)
                {
                    playerTalents.arrowHitTargetEvent(collision.gameObject);
                }
            }
        }
        else
        {
            //arrow does the damage to the enemy
            if (collision.gameObject.tag == "Player")
            {
                float damage = getDamage();
                bool crit = getCrit();
                
                collision.GetComponent<Player_Combat>().takeDamage(damage / 2f, crit, null); 
            }
        }

    }
}
