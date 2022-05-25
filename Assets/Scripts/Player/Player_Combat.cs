using Com.LuisPedroFonseca.ProCamera2D;
using Mirror;
using System;
using System.Collections;
using UnityEngine;


//This class hnadles combat related function, it has many connections because it must get input, check buffs, check sword hitbox,
//do animations, set movement speed modifiers and direction, send events to talents class
public class Player_Combat : NetworkBehaviour
{
    [SyncVar]
    public bool attacking; //true if we are doing any attack/action

    //sword
    public bool sword_attack;
    [SerializeField]
    private float swordHitRange = 1f;

    //bow
    private bool bow_attack;
    public GameObject arrowPrefab;
    private float arrowVelocity = 10f;

    //staff
    private bool staff_attack;

    //shield functionality
    [SerializeField]
    [SyncVar]
    private bool holding_shield;
    public bool getholdingshield()
    {
        return holding_shield;
    }
    public bool parry_oppurtunity; //true during parry oppurtunity

    [SerializeField]
    [SyncVar]
    private bool shieldTired;
    [SerializeField]
    private float shieldCoolTimer, shieldCoolTimerTotal = 1.5f;
    [SerializeField]
    private float shieldHoldTimer, shieldHoldTimerTotal = 3f;
    [SerializeField]
    private float parryTimer, parryTimerTotal = 0.6f;
    public struct shieldInformation
    {
        public bool holding_shield;
        public bool shield_Tired;
        public float shieldCoolTimer, shieldCoolTimerTotal;
        public float shieldHoldTimer, shieldHoldTimerTotal;

        public shieldInformation(bool holding, bool tired, float shieldcooltimer, float shieldcooltimertotal, float shieldholdtimer, float shieldholdtimertotal)
        {
            holding_shield = holding;
            shield_Tired = tired;
            shieldCoolTimer = shieldcooltimer;
            shieldCoolTimerTotal = shieldcooltimertotal;
            shieldHoldTimer = shieldholdtimer;
            shieldHoldTimerTotal = shieldholdtimertotal;
        }
    }
    public shieldInformation getShieldInfo()
    {
        shieldInformation info = new shieldInformation(holding_shield, shieldTired, shieldCoolTimer, shieldCoolTimerTotal, shieldHoldTimer, shieldHoldTimerTotal);
        return info;
    }



    //health
    [SyncVar]
    public float health;
    [Command]
    void CmdChangeHealth(float amt)
    {
        health = health + amt;
    }
    private float maxHealth;
    public float getHealth()
    {
        return health;
    }
    public float getMaxHealth()
    {
        return maxHealth;
    }

    //References
    private Player_Buffs playerBuff; // buffs
    private Player_Input playerInput; //get input for skills and attacks
    private Player_HitDetection playerhd; //sword attack hit check
    private Player_Animation playeranimations;  //attack animation, shield animation
    private Player_Movement playerMove; //set slow, get direction
    private Player_Talents playerTalents; //to send events on combat related things

    private Player_Skills skillmanager; //check input and send input to skills
    private NumberPopupManager numpopup;

    private Player_Inventory inv;


    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    //Grab refernces
    //get health from inventory
    void Start()
    {
        try
        {
            playerTalents = this.GetComponent<Player_Talents>();
            playerBuff = this.GetComponent<Player_Buffs>();
            skillmanager = this.GetComponent<Player_Skills>();
            inv = this.GetComponent<Player_Inventory>();
            playerMove = this.GetComponent<Player_Movement>();
            playerInput = this.GetComponent<Player_Input>();
            playerhd = this.GetComponent<Player_HitDetection>();
            playeranimations = this.GetComponent<Player_Animation>();

            numpopup = GameObject.Find("PopupNumberManager").GetComponent<NumberPopupManager>();
        }
        catch (Exception e)
        {
            Debug.LogError("There was a problem finding the necessary dependents.");
            Debug.LogError(e);
        }

        maxHealth = inv.getMaxHP();
        health = maxHealth;
    }
    //do action and skills from input
    void FixedUpdate()
    {
        if (hasAuthority)
        {
            doAction(playerInput.getAction());
            doSkills(playerInput.getSkill());
        }


    }
    //Synchronize attack state across network
    [Command]
    public void CmdsetAttackState(bool attackingBool)
    {
        attacking = attackingBool;
    }

    [Client]
    private void doSkills(float[] skills)
    {
        if (skills[0] > 0)
        {
            skillmanager.useSkill1();
        }
        if (skills[1] > 0)
        {
            skillmanager.useSkill2();
        }
    }

    [Client]
    //can only pick 1 action at a time so check sword then bow then shield (MUST ADD MAGICSTAFF) TODO
    private void doAction(float[] actions)
    {
        float swordAttack = actions[0];
        float bowAttack = actions[1];
        float shieldMove = actions[2];
        float staffAttack = actions[3];

        if (swordAttack > 0 && shieldMove == 0)
        {
            if (!sword_attack && !bow_attack && !staff_attack)
            {
                StartCoroutine(swordSwing());
                playeranimations.setSwordSwingAnimation();
            }
        }
        else if (bowAttack > 0 && shieldMove == 0)
        {
            if (!bow_attack && !sword_attack && !staff_attack)
            {
                StartCoroutine(bowShot());
                playeranimations.setBowAnimation();
            }
        }
        else if (staffAttack > 0 && shieldMove == 0)
        {
            if (!staff_attack && !bow_attack && !sword_attack)
            {
                StartCoroutine(magicStaffAttack());
                playeranimations.setStaffAnimation();
            }

        }

        if (sword_attack || bow_attack || staff_attack)
        {
            manageShieldFunctionality(0);
        }
        else
        {
            manageShieldFunctionality(shieldMove);
            playeranimations.updateAnimations(playerMove.getRunning(), holding_shield);
        }

    }

    //shield state machine
    [Command]
    private void CmdUpdateHoldingShield(bool value)
    {
        holding_shield = value;
    }
    private void manageShieldFunctionality(float shieldMove)
    {
        if (!shieldTired)
        {
            if (shieldMove > 0)
            {
                if (!holding_shield)
                {
                    //this is first frame
                    parry_oppurtunity = true; // do a countdown then turn parry_oppurtunity to false, and in take damage need to check if enenmy is doing parry then take stun
                }
                CmdUpdateHoldingShield(true);
                //make player slower
                playerMove.setSlow(true);
            }
            else
            {
                parry_oppurtunity = false;
                parryTimer = parryTimerTotal;

                if (holding_shield)
                {
                    //last frame of holding shield
                    //subtract stamina chunk
                    shieldHoldTimer -= 0.5f;
                    if (shieldHoldTimer <= 0f)
                    {
                        shieldTired = true;
                        shieldHoldTimer = shieldHoldTimerTotal;
                    }
                }
                CmdUpdateHoldingShield(false);
                playerMove.setSlow(false);
            }
        }

        if (parry_oppurtunity)
        {
            if (parryTimer >= 0f)
            {
                parryTimer -= 1f * Time.deltaTime;
            }
            else
            {
                parry_oppurtunity = false;
                parryTimer = parryTimerTotal;
            }
        }

        //update shield timers
        if (!holding_shield)
        {
            if (shieldHoldTimer < shieldHoldTimerTotal)
            {
                shieldHoldTimer += 0.5f * Time.deltaTime;
            }

        }

        if (holding_shield && !shieldTired)
        {
            if (shieldHoldTimer >= 0f)
            {
                shieldHoldTimer -= 1f * Time.deltaTime;
            }
            else
            {
                shieldTired = true;
                shieldHoldTimer = shieldHoldTimerTotal;
                CmdUpdateHoldingShield(false);
            }
        }

        if (shieldTired)
        {
            playerMove.setSlow(true);
            if (shieldCoolTimer >= 0f)
            {
                shieldCoolTimer -= 1f * Time.deltaTime;
            }
            else
            {
                shieldTired = false;
                shieldCoolTimer = shieldCoolTimerTotal;
            }
        }
    }


    //Fire bow shot, plays animation, spawns arrow, and sets the arrow details to 
    private IEnumerator bowShot()
    {
        bow_attack = true; //set bool

        playerMove.setLockForTime(1f); //freeze player

        yield return new WaitForSeconds(1f); // delay to sync arrow with animation

        CmdSpawnArrow();
       

        //lead out time
        playerMove.setLockForTime(0.3f);
        yield return new WaitForSeconds(0.3f); 

        bow_attack = false; //set bool
    }

    [Command]
    void CmdSpawnArrow()
    {
        //CREATE ARROW
        GameObject arrowInstance = GameObject.Instantiate(arrowPrefab);//instantiate
        //set arrow details
        float[] charStats = new float[7];
        charStats = inv.getCharacterStats();
        arrowInstance.GetComponent<ArrowDetails>().setDetails(charStats[2], (double)charStats[4], (double)charStats[5]);

        //POSITION AND GIVE VELOCITY
        arrowInstance.transform.position = this.transform.position;
        Vector2 dir = new Vector2(this.transform.localScale.x, 0f);
        if (dir == Vector2.right)
        {
            arrowInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(arrowVelocity, 0);
            arrowInstance.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            arrowInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(-arrowVelocity, 0);
            arrowInstance.GetComponent<SpriteRenderer>().flipX = true;
        }
        arrowInstance.GetComponent<ArrowDetails>().playerTalents = playerTalents;

        NetworkServer.Spawn(arrowInstance);
    }

    private IEnumerator magicStaffAttack()
    {
        staff_attack = true;
        yield return new WaitForSeconds(0.5f);
        staff_attack = false;
    }

    [Client]
    //swing, play animation, find collider, damage each collider
    private IEnumerator swordSwing()
    {
        sword_attack = true; //set bool

        yield return new WaitForSeconds(0.25f);

        //find a collider to do damage
        RaycastHit2D hits = playerhd.swordAttack(swordHitRange);

        if (hits.collider != null)
        {
            if (hits.collider.gameObject.tag == "Enemy")
            {
                CmddoDamage(hits.collider.gameObject, 1); //do damage to each collider
            }
        }

        playerMove.setLockForTime(0.5f); //slow while attacking
        yield return new WaitForSeconds(0.5f); //lead out time
        sword_attack = false;
    }

    [Command]
    private void CmddoDamage(GameObject target, int weapontype) //1 -> sword damage, 2-> bow damage, 3-> magic staff damage
    {
        ProCamera2DShake.Instance.Shake(0);
        float[] updatedstats  = playerBuff.calculateDamageWithBuffs(weapontype);
        float damage = updatedstats[0];
        float critChance = updatedstats[1];
        float critDamage = updatedstats[2];

        float totalDamage = 0;
        //calculate crit
        float rollDice = UnityEngine.Random.value;
        if (rollDice < (float)critChance / 100f)
        {
            //ITS A CRIT
            //how much extra damage?
            totalDamage = damage + damage * ((float)critDamage / 100f); //turn to 


            try
            {
                Enemy_Generic enemy = target.GetComponent<Enemy_Generic>();
                enemy.takeDamage(totalDamage, false, this.transform.position.x - target.transform.position.x);
            }
            catch (Exception e)
            {
                Debug.LogError("There was a problem finding player do damage target class.");
                Debug.LogError(e);
            }
            
        }
        else
        {
            //NOT A CRIT
            totalDamage = damage;

            
            try
            {
                Enemy_Generic enemy = target.GetComponent<Enemy_Generic>();
                enemy.takeDamage(totalDamage, false, this.transform.position.x - target.transform.position.x);
            }
            catch(Exception e)
            {
                Debug.LogError("There was a problem finding player do damage target class.");
                Debug.LogError(e);
            }
           
           
        }


    }

    [Command]
    private void CmdStunEnemy(GameObject enemy)
    {
        enemy.GetComponent<Enemy_Generic>().stunEnemy();
        print("parry");
    }

    [ClientRpc]
    //compares our armour against their damage and evaluates the damage we take
    public void takeDamage(float damageValue, bool crit, GameObject damager)
    {
        ProCamera2DShake.Instance.Shake(1);
        //calculate armour with buffs/debuffs
        float armour = playerBuff.calculateArmourWithBuffs();

        //check if armour holds up against damage
        float shieldArmour = inv.getShieldArmour();
        float healthDamage;
        if (holding_shield)
        {
            healthDamage = damageValue - (armour + shieldArmour);

        }
        else
        {
            healthDamage = damageValue - armour;
        }


        if (healthDamage <= 0 && holding_shield)
        {
            //Attack blocked!
            //Show a number pop up saying blocked
            numpopup.spawnBlockedPopup(this.transform);

            //knock back enemy / stun, check for enemy class from gameobject
            if (damager != null && parry_oppurtunity)
            {
                if (damager.GetComponent<Enemy_Generic>() != null)
                {
                    //need to call cmd on player, then that calls stun on enemy
                    CmdStunEnemy(damager);
                }
            }


        }
        else if (healthDamage <= 0)
        {
            //Not blocked!, but no damage
            //Show damage number pop up
            //numpopup.spawnHitPopup(crit, 0f, this.transform, true);
        }
        else
        {
            //Show damage number pop up
            numpopup.spawnHitPopup(crit, healthDamage, this.transform, true);
            health -= healthDamage; //CmdChangeHealth(-healthDamage);

            //need to check if that killed us, (method in void update)
        }

    }
    //this method is used when applying debuff damage poison
    public void applyHealthDamage(float damageValue)
    {
        health -= damageValue;// CmdChangeHealth(-damageValue);
        numpopup.spawnHitPopup(false, damageValue, this.transform, true);

    }

    //heal the player (up to max health)
    public void healPlayer(float amt)
    {
        health += amt;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        numpopup.spawnHealPopup(amt, this.transform);
    }

}

