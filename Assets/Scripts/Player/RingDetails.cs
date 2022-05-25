using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//This class handles the ring details for physical skill objects (enemy and player uses this)
public class RingDetails : NetworkBehaviour
{
    //contains type, number or percent bool, amont, duration
    [SyncVar]
    public BuffDebuff buff;


    //ring lifespan
    [SyncVar]
    public float duration = 10f;

    [SyncVar]
    public bool playerSkill; //true if this is a player object skill, false if its an enemy

    [SyncVar]
    public uint sourceID;
    void Start()
    {
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

        if (collider.gameObject.tag == "Enemy" && !playerSkill)
        {
            //collider.GetComponent<Enemy_Generic>().addBuffDebuff(buff.buffDebuff, buff.totalDuration);
        }


    }
}

