using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player_Input : NetworkBehaviour
{
    [SerializeField]
    private float horizontalMove;
    [SerializeField]
    private float primaryAttack;
    [SerializeField]
    private float secondaryAttack;
    [SerializeField]
    private float tertiaryAttack;
    [SerializeField]
    private float jump;
    [SerializeField]
    private float skill1;
    [SerializeField]
    private float skill2;
    [SerializeField]
    private float shield;
    [SerializeField]
    private float interact;

    void Update()
    {
        if (isLocalPlayer == false) { return; }
        getInput();
    }
    private void getInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        jump = Input.GetAxis("Jump");

        primaryAttack = Input.GetAxis("Fire1"); //sword
        secondaryAttack = Input.GetAxis("Fire2"); //bow
        tertiaryAttack = Input.GetAxis("Fire3"); //magic staff
        shield = Input.GetAxis("Block"); //shield
        skill1 = Input.GetAxis("Skill1"); //skill 1
        skill2 = Input.GetAxis("Skill2"); //skill 2

        interact = Input.GetAxis("Interact");
    }

    public float getHorizontalMove()
    {
        return horizontalMove;
    }

    public float getJump()
    {
        return jump;
    }

    public float[] getInteract()
    {
        float[] temp = { interact, primaryAttack };
        return temp;
    }

    public float[] getAction()
    {
        float[] actions = { primaryAttack, secondaryAttack, shield, tertiaryAttack };
        return actions;
    }

    public float[] getSkill()
    {
        float[] skill = { skill1, skill2 };
        return skill;
    }
    
}