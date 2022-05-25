using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Com.LuisPedroFonseca.ProCamera2D;
using Mirror;

//This class handles appling movement to the player
public class Player_Movement : NetworkBehaviour
{
    private Player_Input playerInput; //gets input from user
    private Rigidbody2D rb; //the player rigidbody to do the movement on
    private Player_HitDetection playerdetect; //for jump methods
    public bool playerDetectEnabled;

    [Header("Movement Variables")]
    [Tooltip("This value controls the force exerted by a jump.")]
    [SerializeField]
    private float jump_force;
    [Tooltip("This value controls the speed the player moves horizontally.")]
    [SerializeField]
    private float movement_speed;


    [Header("Movement Stati")]
    [SerializeField]
    private bool running;
    [SerializeField]
    [SyncVar]
    private Vector2 runDirection;

    //Getters
    public Vector2 getDirection()
    {
        return runDirection;
    }
    public bool getRunning()
    {
        return running;
    }

    public bool slowed;
    public void setSlow(bool s)
    {
        slowed = s;
    }
    public bool movementLock;
    public void setLockForTime(float t)
    {
        StartCoroutine(lockMovement(t));
    }
    public IEnumerator lockMovement(float t)
    {
        movementLock = true;
        yield return new WaitForSeconds(t);
        movementLock = false;
    }


    void Awake()
    {
        try
        {
            ProCamera2D.Instance.AddCameraTarget(this.transform);
            playerInput = this.GetComponent<Player_Input>();
            if (playerDetectEnabled) { playerdetect = this.GetComponent<Player_HitDetection>(); }
            
            rb = this.GetComponent<Rigidbody2D>();
        }
        catch (Exception e)
        {
            Debug.LogError("There was a problem finding the necessary dependents.");
            Debug.LogError(e);
        }

    }



    void FixedUpdate()
    {
        if (!hasAuthority) { return; }
        move_Player(playerInput.getHorizontalMove());

        if (playerDetectEnabled) { CmddoJump(playerInput.getJump()); }
        

    }
    

    private void move_Player(float horizontalInput)
    {
        if (slowed || movementLock)
        {
            rb.velocity = new Vector2(horizontalInput * 1f, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * movement_speed, rb.velocity.y);
        }
        

        if (horizontalInput > 0)
        {
            CmdSetRunDirection(Vector2.right);
            running = true;
        }
        else if(horizontalInput < 0)
        {
            CmdSetRunDirection(Vector2.left);
            running = true;
        }
        else
        {
            running = false;
        }
    }

    [Command]
    void CmdSetRunDirection(Vector2 value)
    {
        runDirection = value;
    }


    [Command]
    void CmddoJump(float jump)
    {
        if (!hasAuthority) { return;  }
        if (jump > 0)
        {
            if (playerdetect.isGrounded())
            {
                if (slowed || movementLock)
                {
                    RpcJump(jump_force / 2f);
                }
                else
                {
                    RpcJump(jump_force);
                }
               
            }
            if (playerdetect.isOnLadder())
            {
                RpcJump(7f);
            }
        }
        
    }
    [ClientRpc]
    void RpcJump(float jump)
    {
        rb.velocity = new Vector2(rb.velocity.x, jump);
    }
}
