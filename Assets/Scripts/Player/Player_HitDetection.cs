using System.Collections;
using UnityEngine;
using System;
using Mirror;

// This class handles the necessary collision and trigger detection
// The functionality this class handles ranges from interfacing with the inventory, interacting with latters and puzzle pieces, and raycast checking the sword collision range
public class Player_HitDetection : NetworkBehaviour
{

    [SerializeField]
    private bool grounded = false;
    [SerializeField]
    private bool onLadder = false;

    //getters
    public bool isGrounded()
    {
        return grounded;
    }
    public bool isOnLadder()
    {
        return onLadder;
    }

    //For attack raycast
    private float swordAttackRayDis;

    [SerializeField]
    private LayerMask enemyMask;

    private float entryX, exitX; //these variables control the pressure plate interaction

    //Controls interacting with objects, unlock door use switch
    [SerializeField]
    private bool canInteract = true;

    //References
    private Player_Movement playerMove; //to get the direction for sword attack check
    private Player_Input playerinput; //for interact
    private Player_Inventory inv; //to get key details when checking for door opening
    private Player_Animation playeranim; //for particle effect
    private Player_Buffs playerBuffs;
    void Awake()
    {
        try
        {
            playerinput = this.GetComponent<Player_Input>();
            playerMove = this.GetComponent<Player_Movement>();
            inv = this.GetComponent<Player_Inventory>();
            playeranim = this.GetComponent<Player_Animation>();
            playerBuffs = this.GetComponent<Player_Buffs>();
        }
        catch (Exception e)
        {
            Debug.LogError("There was a problem finding the necessary dependents for Player Hit Detection");
            Debug.LogError(e);
        }

    }

    void OnParticleCollision(GameObject collision)
    {
        if (collision.gameObject.tag == "LootDrop")
        {
            if (collision.gameObject.GetComponentInParent<LootDropDetails>()!=null)
            {
                collision.gameObject.GetComponentInParent<LootDropDetails>().startedFade = true;
            }
            
        }
    }
   
    //Ground detection ----------
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
          
            playeranim.dustEffect();
            
            grounded = true;
           
        }
       
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }

    //Puzzle interaction ------------
    private IEnumerator interactCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(0.15f);
        canInteract = true;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //auto pickup
        if (collision.tag == "Key")
        {
            //get key details
            KeyDetails.keyColor c = collision.gameObject.GetComponent<KeyDetails>().color;
            if (c == KeyDetails.keyColor.white)
            {
                inv.keys[0] = true;
                GameObject.Destroy(collision.gameObject);
            }
            if (c == KeyDetails.keyColor.red)
            {
                inv.keys[2] = true;
                GameObject.Destroy(collision.gameObject);
            }
            if (c == KeyDetails.keyColor.blue)
            {
                inv.keys[5] = true;
                GameObject.Destroy(collision.gameObject);
            }
            if (c == KeyDetails.keyColor.purple)
            {
                inv.keys[1] = true;
                GameObject.Destroy(collision.gameObject);
            }
        }

        //puzzle detection
        if (collision.tag == "PressureSwitch")
        {
            entryX = this.gameObject.transform.position.x - collision.gameObject.transform.position.x;
            collision.gameObject.GetComponent<SwitchLogic>().onOff = !collision.gameObject.GetComponent<SwitchLogic>().onOff;
        }

        if (collision.tag == "PressurePlate")
        {
            collision.gameObject.GetComponent<SwitchLogic>().onOff = true;
        }

    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PressureSwitch")
        {
            exitX = this.gameObject.transform.position.x - collision.gameObject.transform.position.x;
            if (entryX < 0 && exitX > 0)
            {
                entryX = 0f;
                exitX = 0f;
            }
            else if (entryX > 0 && exitX < 0)
            {
                entryX = 0f;
                exitX = 0f;
            }
            else
            {
                //so reset switch
                collision.gameObject.GetComponent<SwitchLogic>().onOff = !collision.gameObject.GetComponent<SwitchLogic>().onOff;
            }
        }
        if (collision.tag == "Ladder")
        {
            onLadder = false;
        }

    }

    [Client]
    void OnTriggerStay2D(Collider2D collision)
    {
        //need to pickup
        if ((playerinput.getInteract()[0] > 0 || playerinput.getInteract()[1] > 0) && canInteract)
        {
            //click button to select?
            if (collision.tag == "Door")
            {

                //get door details
                DoorDetails.doorColor c = collision.gameObject.GetComponent<DoorDetails>().color;
                if (c == DoorDetails.doorColor.white)
                {
                    //do we have the key?
                    if (inv.keys[0])
                    {
                        //we have white key
                        //open the door
                        GameObject.Destroy(collision.gameObject);
                        inv.keys[0] = false;
                    }
                }
                if (c == DoorDetails.doorColor.red)
                {
                    //do we have the key?
                    if (inv.keys[2])
                    {
                        //we have red key
                        //open the door
                        GameObject.Destroy(collision.gameObject);
                        inv.keys[2] = false;
                    }
                }
                if (c == DoorDetails.doorColor.blue)
                {
                    //do we have the key?
                    if (inv.keys[5])
                    {
                        //we have red key
                        //open the door
                        GameObject.Destroy(collision.gameObject);
                        inv.keys[5] = false;
                    }
                }
                if (c == DoorDetails.doorColor.purple)
                {
                    //do we have the key?
                    if (inv.keys[1])
                    {
                        //we have red key
                        //open the door
                        GameObject.Destroy(collision.gameObject);
                        inv.keys[1] = false;
                    }
                }
                StartCoroutine(interactCooldown());
            }


        }

        if (collision.tag == "Ladder")
        {
            onLadder = true;
        }

       

        if ((playerinput.getInteract()[0] > 0 || playerinput.getInteract()[1] > 0) && canInteract)
        {
            if (collision.tag == "Switch")
            {
                //interact
                collision.gameObject.GetComponent<SwitchLogic>().onOff = !collision.gameObject.GetComponent<SwitchLogic>().onOff;
                StartCoroutine(interactCooldown());
            }
            if (collision.tag == "Book")
            {
                //interact
                if (!collision.gameObject.GetComponent<BookDetails>().opened)
                {
                    collision.gameObject.GetComponent<BookDetails>().openBook();
                }
                else
                {
                    collision.gameObject.GetComponent<BookDetails>().nextPage();
                }
                StartCoroutine(interactCooldown());
            }

           
        }

       
        if (collision.tag == "Ring")
        {
            if (collision.GetComponent<RingDetails>().playerSkill)
            {
                BuffDebuff buff = collision.GetComponent<RingDetails>().buff;
                playerBuffs.addBuffDebuff(buff, collision.GetComponent<RingDetails>().sourceID);
            }
           
        }


    }


    //This function checks in front of the player for enemies to do damage to when swinging sword
    public RaycastHit2D swordAttack(float hitrange)
    {
        //what direction are we facing?
        Vector2 dir = new Vector2(this.transform.localScale.x, 0f); 
        RaycastHit2D hits = Physics2D.CircleCast(transform.position, 1f, dir, hitrange, enemyMask);
        return hits;
    }

}

