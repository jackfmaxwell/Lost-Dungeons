using UnityEngine;
using System;
using Mirror;


//
public class Player_Animation : NetworkBehaviour
{
    //Animators
    public Animator player, helmet, chestplate, boot, weapon;
    //Objects
    public GameObject playerObj, helmetObj, chestplateObj, bootObj, weaponObj;

    private const string PLAYER_SHIELD="Player_Shield", PLAYER_RUN="Player_Run", PLAYER_IDLE="Player_Idle", PLAYER_SWORD="Player_Sword", PLAYER_BOW="Player_Bow", PLAYER_STAFF="Player_Staff";
    private const string HELMET_SHEILD = "Helmet_Shield", HELMET_RUN = "Helmet_Run", HELMET_IDLE = "Helmet_Idle", HELMET_SWORD = "Helmet_Sword", HELMET_BOW = "Helmet_Bow", HELMET_STAFF = "Helmet_Staff";
    private const string CHESTPLATE_SHEILD = "Chestplate_Shield", CHESTPLATE_RUN = "Chestplate_Run", CHESTPLATE_IDLE = "Chestplate_Idle", CHESTPLATE_SWORD = "Chestplate_Sword", CHESTPLATE_BOW = "Chestplate_Bow", CHESTPLATE_STAFF = "Chestplate_Staff";
    private const string BOOT_SHIELD = "Boot_Shield", BOOT_RUN = "Boot_Run", BOOT_IDLE = "Boot_Idle", BOOT_SWORD = "Boot_Sword", BOOT_BOW = "Boot_Bow", BOOT_STAFF = "Boot_Staff";
    private const string WEAPON_SHIELD = "Weapon_Shield", WEAPON_RUN = "Weapon_Run", WEAPON_IDLE = "Weapon_Idle", WEAPON_BOW = "Weapon_Bow", WEAPON_SWORD = "Weapon_Sword", WEAPON_STAFF = "Weapon_Staff";

    //References
    private Player_Movement playerMove; // This class gets us info on the player state (running, direction, ...)
    private Player_Combat playerCombat; // This class gets us info on the player attack state (sword swing, bow shot ...)

    void Awake()
    {
        try
        {
            playerMove = this.GetComponent<Player_Movement>();
            playerCombat = this.GetComponent<Player_Combat>();
        }
        catch(Exception e)
        {
            Debug.LogError("There was a problem finding the necessary dependents for Player Animation.");
            Debug.LogError(e);
        }
    }

    //Dust effect
    public GameObject dust;
   
    public void dustEffect()
    {
        GameObject dustObj = GameObject.Instantiate(dust);
        dustObj.transform.position = this.transform.position + new Vector3(0f, -0.75f, 0f);
    }
    
    public void setSwordSwingAnimation()
    {
        player.Play(PLAYER_SWORD);
        weapon.Play(WEAPON_SWORD);
        helmet.Play(HELMET_SWORD);
        chestplate.Play(CHESTPLATE_SWORD);
        boot.Play(BOOT_SWORD);
    }
    public void setBowAnimation()
    {
        player.Play(PLAYER_BOW);
        weapon.Play(WEAPON_BOW);
        helmet.Play(HELMET_BOW);
        chestplate.Play(CHESTPLATE_BOW);
        boot.Play(BOOT_BOW);
    }
    public void setStaffAnimation()
    {
        player.Play(PLAYER_STAFF);
        weapon.Play(WEAPON_STAFF);
        helmet.Play(HELMET_STAFF);
        chestplate.Play(CHESTPLATE_STAFF);
        boot.Play(BOOT_STAFF);
    }

    public void updateAnimations(bool running, bool holdingShield)
    {
        if (holdingShield)
        {
            //since this is a one frame animation it can be played loop instantly
            player.Play(PLAYER_SHIELD);
            weapon.Play(WEAPON_SHIELD);
            helmet.Play(HELMET_SHEILD);
            chestplate.Play(CHESTPLATE_SHEILD);
            boot.Play(BOOT_SHIELD);
        }
        else
        {

            //Set all the bools 
            if (running)
            {

                player.Play(PLAYER_RUN);
                weapon.Play(WEAPON_RUN);
                helmet.Play(HELMET_RUN);
                chestplate.Play(CHESTPLATE_RUN);
                boot.Play(BOOT_RUN);

            }
            else
            {
                player.Play(PLAYER_IDLE);
                weapon.Play(WEAPON_IDLE);
                helmet.Play(HELMET_IDLE);
                chestplate.Play(CHESTPLATE_IDLE);
                boot.Play(BOOT_IDLE);

            }
        }
    }
   

    public void flipSprite(Vector2 runDirection)
    {
        //Flip all the sprites and fix the collider offest
        if (runDirection == Vector2.right)
        {
            this.transform.localScale = new Vector3(1f, 1f, 1);
        }
        if (runDirection == Vector2.left)
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1);
        }
    }

    void FixedUpdate()
    {
        if (!playerCombat.attacking)
        {
            flipSprite(playerMove.getDirection());
        }
       
    }
       
    

}
