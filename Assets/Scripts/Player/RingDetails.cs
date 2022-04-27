using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//This class handles the ring details for physical skill objects (enemy and player uses this)
public class RingDetails : NetworkBehaviour
{
    //contains type, number or percent bool, amont, duration
    public Player_Buffs.ActiveBuffDebuff buff;

    //ring lifespan
    public float duration=10f;

    public bool tick; //acts like invoke repeating but for checking the collider status 

    public bool playerSkill; //true if this is a player object skill, false if its an enemy

    
    void Start()
    {
        InvokeRepeating("tickFunction", 0.5f, 0.5f);

        StartCoroutine(inializeCollider());
    }
    //the issue is that when triggers are spawned on colliders they dont count the stay in collider until after the trigger is refreshed
    //by refreshed i mean that the boxcollider must be disabled then enabled
    private IEnumerator inializeCollider()
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<BoxCollider2D>().enabled = true;
    }

    //timer for the ring life
    void Update()
    {
        if (duration > 0f)
        {
            duration -= 1f * Time.deltaTime;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
            
        }
    }
    void OnTriggerStay2D(Collider2D collider)
    {
        //the issue is that enemys cannot collide with enemies
        if (tick)
        {
            //check player collider and enemy collider
            if (collider.gameObject.tag == "Player" && playerSkill)
            {
                collider.GetComponent<Player_Buffs>().addBuffDebuff(buff.buffDebuff, buff.totalDuration);

            }

            if (collider.gameObject.tag == "Enemy" && !playerSkill)
            {
                collider.GetComponent<Enemy_Generic>().addBuffDebuff(buff.buffDebuff, buff.totalDuration);
                

            }

        }
    }

    //used as makeshift invoke repeating on triggerstay2d
    private void tickFunction()
    {
        if (tick)
        {
            tick = false;
        }
        else
        {
            tick = true;
        }
       
    }
}
