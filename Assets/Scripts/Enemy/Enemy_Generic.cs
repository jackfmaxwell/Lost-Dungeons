using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using Mirror;

public class Enemy_Generic : NetworkBehaviour
{
    [Header("Pathfinding")]
    //These methods and lists allow for enemy to keep track of nearby targets and nearby allies
    public Transform topPriorityTarget; //the target the enemy is following
    public List<Transform> targetList = new List<Transform>();
    public void spottedTarget(Transform target)
    {
        //is it already in list?
        if (targetList.Contains(target))
        {
            //do nothing
            return;
        }
        else
        {
            targetList.Add(target);
        }
       
    }
    public void lostSightOfTarget(Transform target)
    {
        targetList.Remove(target);
        
    }

    public List<Transform> nearbyAllies;
    public void spottedAlly(Transform ally)
    {

        //is it already in list?
        if (nearbyAllies.Contains(ally))
        {
            //do nothing
            return;
        }
        else
        {
            nearbyAllies.Add(ally);
        }

    }
    public void lostSightOfAlly(Transform ally)
    {
        nearbyAllies.Remove(ally);
    }

    [Tooltip("How often the path is updated in seconds (cant be changed runtime)")]
    protected float pathUpdateSeconds = 0.5f;
    protected Path path; //pathfinding path object
    protected int currentWaypoint = 0; //current waypoint index
    //---------------------------------------------------------


    [Header("Physics")]
    public float speed = 3f; //movement speed
    [Tooltip("How close we need to get to waypoints for it to count and to let us go toward the next one")]
    protected float nextWaypointDistance = 1f;
    [Tooltip("How high the vector must be pointing up for us to do a jump")]
    protected float jumpNodeHeightRequirement = 0.95f;
    [Tooltip("How high we jump")]
    public float jumpModifier = 10f;
    [Tooltip("How long we wait after jumping to jump again")]
    public float jumpCooldown = 0.5f;
    [Tooltip("The strengh of the A* path needed for AI to treat it as movement")]
    protected float directionOverhead = 0.2f;
    [Tooltip("The desired direction the AI wants to move")]
    [SyncVar]
    public Vector2 desiredMovement;
    //This is used to keep a barrier between what the AI wants to do and the actual physics of the character. 
    //This allows easier knockback and ect. May need multiple vectors for the different behaviour decisions then pick 
    //a vector that is most desirable


    [Header("Behaviour Bools")]
    public bool followEnabled = true; //will the AI follow the path given by A*
    public bool plotPath = true; //Will the AI plot a path to the target
    [SyncVar]
    public bool stunned = false;
    [Space(15)]
    public bool jumpEnabled = true; //Will the AI jump if the path asks for it
    public bool directionLookEnabled = true; //Will the AI face the target
    public LayerMask stairCheck; //to check for stairs (ground) while walking backward
    [Header("Feedback Bools")]
    public bool jumping = false; //true when jumping
    public bool attacking = false; //true when attacking
    public bool isGrounded; //true when on the ground (collided with "Ground" Collider)
    public bool knockedBack = false; //true when knock back called, halts velocity update
    public bool backingUp = false; //true when the enemy is backing up from pri target
    [Header("Behaviour Variables")]
    //Handles backing up (distance and timer)
    public float backUpDistance = 2f;
    public float backuptimer, backuptimertotal = 1.5f;
    [Tooltip("This variable controls how close to the pri target AI wants to get")]
    public float desiredPromixtyToTarget = 1f; //should increase value after attacking to stagger multiple enemys and let others attack

    //References
    [Header("Animation/Visuals")]
    protected EnemyAnimationManager enemyAnimations;
    protected SpriteRenderer sprite;
    private NumberPopupManager numberPopupManager; //reference to the number p
    public LootDropManager lootDrops;

    [Header("Health and Stats")]
    public EnemyObjectScript enemyDetails;
    [SyncVar]
    public float currentHealth;

    //References
    protected Seeker seeker;
    protected Rigidbody2D rb;

    //buff system and list
    [Serializable]
    public class ActiveBuffDebuff
    {
        public BuffDebuff buffDebuff;
        public float duration; // counts down
        public float totalDuration;
        public uint id;

        public ActiveBuffDebuff(BuffDebuff buffD, uint sourceID)
        {
            buffDebuff = buffD;
            duration = buffD.buffDuration;
            totalDuration = duration;
            id = sourceID;
        }
    }
    public List<ActiveBuffDebuff> activeBuffsDebuffs = new List<ActiveBuffDebuff>();

    //This method initializes reference classes, and initalizes the target and allies lists
    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        enemyAnimations = GetComponent<EnemyAnimationManager>();
        numberPopupManager = GameObject.Find("PopupNumberManager").GetComponent<NumberPopupManager>();

        currentHealth = enemyDetails.health;

        //initialize the target and allies list
        targetList = new List<Transform>();
        nearbyAllies = new List<Transform>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds); // set A* update path method to repeat every pathupdate seconds (0.5 sec)
        InvokeRepeating("applyBuffDebuffEffect", 0f, 1f);

        //slightly stagger the following variables to ensure no enemies overlap
        //desiredProximityToTarget
        desiredPromixtyToTarget = UnityEngine.Random.Range(0.5f, 1.2f);
    }

    //This method is called in Fixed update
    //sets the topPriorityTarget from the top of our target list
    protected virtual void update_topPriorityTarget()
    {
        if (targetList.Count > 0)
        {
            topPriorityTarget = targetList[targetList.Count - 1];
        }
    }

    //This method returns the distance (pos.x) to the priority target, if there is no priority target then it returns INF
    public float distanceToPriorityTarget()
    {
        if (topPriorityTarget != null)
        {
            float distance = Mathf.Abs(this.transform.position.x - topPriorityTarget.position.x);
            return distance;
        }
        else
        {
            return 99999;
        }
      
    }

    [Command (requiresAuthority =false)]
    void CmdUpdateDesiredMovement(Vector2 vec)
    {
        desiredMovement = vec;
    }
    
    //This method tells the enemy to backup from the pri, its called when the enemy wants to attack the pri but 
    //checks and sees that another enemy is closer. AND its called after the enemy finishes an attack
    protected void backup() //param for distance/time to backup for? or set the protected var in start of sub class?
    {
        if (!stunned)
        {
            backingUp = true;
            //if there is a pri target in view, set desired movement away from pri
            if (topPriorityTarget != null)
            {
                if (this.transform.position.x > topPriorityTarget.transform.position.x + 0.05f)
                {
                    CmdUpdateDesiredMovement(new Vector2(0.5f, 0f));
                    
                }
                else
                {
                    CmdUpdateDesiredMovement(new Vector2(-0.5f, 0f));
                    
                }
            }
        }
       
    }
    
    //This method accompanies backup(), this method is called in FixedUpdate, it stops the enemy backingUp if they get
    //far enough away, or enough time has gone by of them backing up
    protected virtual void backupDistanceChecker()
    {
        //if we  are backing up and can still see the pri target
        if (backingUp && topPriorityTarget!=null)
        {
            //check the distance to the pri target and if we are far enough away: stop backing away
            if (distanceToPriorityTarget() > backUpDistance)
            {
                backuptimer = 0;
                CmdUpdateDesiredMovement(new Vector2(0f, 0f));
                
                backingUp = false;
                followEnabled = true;
            }
        }

        //if the pri target is null, but we are still backing up, check if enough time has gone by
        if (backingUp)
        {
            //count
            if (backuptimer < backuptimertotal)
            {
                backuptimer += 1 * Time.deltaTime;
            }
            //set back to normal
            else
            {
                backuptimer = 0;
                CmdUpdateDesiredMovement(new Vector2(0f, 0f));
                
                backingUp = false;
                followEnabled = true;
                
                
            }
        }
    }

    //This method is called every set time
    protected void chasePriTarget()
    {
        //check how far we are from our target, (may not need to get closer)
        if (followEnabled && topPriorityTarget != null)
        {
            //if we are further than certain distance away then dont chase target
            float distance = distanceToPriorityTarget();
            if (distance > desiredPromixtyToTarget)
            {
                PathFollow();
            }
            //we are close enough, reachedDestination()
            else
            {
                CmdUpdateDesiredMovement(new Vector2(0f, 0f));
                
                reachedDestination();
            }

        }
    }

    public void stunEnemy()
    {
        stunned = true;
        StartCoroutine(stunEffect());
    }
    private IEnumerator stunEffect()
    {
        
        CmdUpdateDesiredMovement(new Vector2(0f, 0f));
        //set to 0 to stop movement (animation specifcally)
        yield return new WaitForSeconds(3f);
        stunned = false;
        //we suspended these functions from normally operating 
        backingUp = false;
        followEnabled = true; 
    }

    //Called every set time
    [ServerCallback]
    protected virtual void FixedUpdate()
    {
        update_topPriorityTarget(); //keep pri target updated

        backupDistanceChecker(); //check counters for backing up

        if (currentHealth <= 0f)
        {
            die();
        }

       

        if (!stunned)
        {
            updateMovement(); //update velocity with the desired movement vector
            chasePriTarget(); //chase pri target if follow enabled, checks if we are close enough to reachDestination()
        }
       
    }

    //This method is called every frame and updates the AI velocity with the desired movement, unless knocked back
    protected void updateMovement()
    {
        //normalize the desired Movement vector
        Vector2 normDirDesire = new Vector2(0f, 0f);
        if (desiredMovement.x != 0)
        {
            normDirDesire = new Vector2(Mathf.Abs(desiredMovement.x) / (desiredMovement.x), 0);
        }
        


        //check if there are external influences on movement (knockback), if there is dont allow AI to do movement
        if (!knockedBack)
        {
            rb.velocity = new Vector2(normDirDesire.x * speed, rb.velocity.y); //apply movement

            //direction look, look at pri target, include buffer of 0.05
            if (topPriorityTarget != null)
            {
                if (directionLookEnabled)
                {
                    if (topPriorityTarget.position.x > this.transform.position.x + 0.05f)
                    {
                        if(isServer)
                            RpcflipSprite(1f);
                    }
                    else if (topPriorityTarget.position.x < this.transform.position.x - 0.05f)
                    {
                        if (isServer)
                            RpcflipSprite(-1f);
                    }
                }
            }
        }

        //Do a check for stairs in the way in retreat, jump if encounter them
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(-1f*this.transform.localScale.x, 0f), 0.45f, stairCheck);
        if (hit.collider!=null)
        {
            StartCoroutine(doJump());
        }

    }

    [ClientRpc]
    void RpcflipSprite(float dir)
    {
        transform.localScale = new Vector3(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    
    private float checkDistanceToAllies()
    {
        float closestDistance=99999;
        foreach(Transform ally in nearbyAllies)
        {
            float distance = Mathf.Abs(this.transform.position.x - ally.position.x);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }
        return closestDistance;
    }

    //This method is called when the AI following the path is close enough to the pri target
    //The logic is implemented in subclasses
    public virtual void reachedDestination()
    {
        //...
    } //MUST BE PUBLIC NOT PROTECTED SO IT CAN BE OVERRIDDEN



    //-----------A* LOGIC---------------
    //Updates the A* path based on the target position
    //This is called every half second, (invoked at start())
    protected void UpdatePath()
    {
        //plot a path to pri target,  if plotPath enabled
        if (seeker.IsDone() && plotPath)
        {
            if (targetList.Count > 0 && topPriorityTarget!=null)
            {
                seeker.StartPath(this.transform.position, topPriorityTarget.position, OnPathComplete);
            }
        }
    }
    //This method is called is chasePriTarget(), called every fixedUpdate(). (If follow enabled)
    //Uses A* pathfinding
    protected void PathFollow()
    {
        //no path to follow
        if (path == null)
        {
            return;
        }

        // Reached end of path, but let distance checker determine reach destination (we usually stop further than finishing the path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }
        
        //Calculate the desired movement from the A* path
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        if(Mathf.Abs(direction.x) < directionOverhead) //0.2 buffer, dont accept desired directions that have strength < 0.2
        {
            direction = new Vector2(0.05f, direction.y); //path direction is too weak, no movment (0.05 to stop jittering)
        }
        //strenght high enough, make determination of left or right
        else
        {
            if (direction.x > 0f) {
                direction = new Vector2(1f, direction.y);
            }
            if(direction.x < 0f)
            {
                direction = new Vector2(-1f, direction.y);
            }
        }
        CmdUpdateDesiredMovement(direction);
       

        

        //Calculate if the A* path wants the AI to jump, if jump enabled and on the ground
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement) //must be high enough strengh on the path
            {
                if (!jumping)
                {
                    StartCoroutine(doJump()); //Do a jump
                }
                
            }
        }

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        
    }
    //Called when the path is complete (called from seeker.Start path)
    protected void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    //-----------A* LOGIC---------------


    //Do a jump with a cooldown
    protected IEnumerator doJump()
    {
        directionLookEnabled = false; //dont change look direction while jumping (prevents look LR spam)
        jumping = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpModifier); //*NOTE: overrides the desired velocity 
        yield return new WaitForSeconds(jumpCooldown);
        jumping = false;
        directionLookEnabled = true;
    }
    
    //called when the enemy dies (health is 0), may be overridden in subclass to implment a different death situation
    [ClientRpc]
    public virtual void die()
    {
        enemyAnimations.deathAnim();
        GameObject.Destroy(this.transform.gameObject);
       // lootDrops.spawnLootDrop(this.transform, 1, LootDropManager.dropType.enemy); //turned off while I fix the loot drops (its null rn)
    } //MUST BE PUBLIC NOT PROTECTED SO IT CAN BE OVERRIDDEN


    //Functions to add and remove buffs
    public void addBuffDebuff(BuffDebuff buffDebuff, uint sourceID)
    {
        //is this buff already in the list?
        bool contained = false;

        foreach (ActiveBuffDebuff buff in activeBuffsDebuffs)
        {
            if (buff.id == sourceID)
            {
                //we already have this buff in the list
                //refresh the duration
                buff.duration = buff.totalDuration;
                contained = true;
            }
        }
        if (!contained)
        {
            ActiveBuffDebuff item = new ActiveBuffDebuff(buffDebuff, sourceID);
            activeBuffsDebuffs.Add(item);
        }

    }
    public void removeBuffDebuff(ActiveBuffDebuff buffDebuff)
    {
        activeBuffsDebuffs.Remove(buffDebuff);
    }

    //invoked in start to loop through buffs and apply any overtime effects
    public void applyBuffDebuffEffect()
    {
        List<ActiveBuffDebuff> toberemoved = new List<ActiveBuffDebuff>();
        foreach (ActiveBuffDebuff item in activeBuffsDebuffs)
        {
            BuffDebuff buff = item.buffDebuff;
            if (buff is BuffDebuff)
            {
                if (buff.overTime)
                {
                    //Get the buff info and apply it
                    if (buff.increaseorDecrease)
                    {
                        //this buff increases the stat
                        if (buff.statToModify == BuffDebuff.stats.health)
                        {
                            heal(buff.amount);
                        }
                    }
                    else
                    {
                        //this buff increases the stat
                        if (buff.statToModify == BuffDebuff.stats.health)
                        {
                            applyHealthDamage(buff.amount);
                        }
                    }
                }

                item.duration -= 1;
                if (item.duration <= 0)
                {
                    toberemoved.Add(item);
                }
            }
        }
        //now remove all the buffs that are in the temp toberemoved from the real activebuffdebuffs
        foreach(ActiveBuffDebuff item in toberemoved)
        {
            activeBuffsDebuffs.Remove(item);
        }
        
    }

    //this method is used when applying debuff damage poison
    public void applyHealthDamage(float damageValue)
    {
        currentHealth -= damageValue;
        numberPopupManager.spawnHitPopup(false, damageValue, this.transform, true);

    }
    //heal the enemy
    public void heal(float amt)
    {
        currentHealth += amt;
        if (currentHealth > enemyDetails.health)
        {
            currentHealth = enemyDetails.health;
        }
        numberPopupManager.spawnHealPopup(amt, this.transform);

    }

    //The damage system functions, these two functions are needed for any enemy/boss
    [ClientRpc]
    public virtual void RpctakeDamage(float healthDamage, bool crit, float facing)
    {
        print("take damage rpc");
        
        enemyAnimations.damageTakeAnim(facing); //start damage animation

        if (healthDamage <= 0)
        {
            //Attack blocked!
            //Show a number pop up saying blocked
            numberPopupManager.spawnBlockedPopup(this.transform);
        }
        else
        {
            //Not blocked!
            //Show damage number pop up
            //Spawn a damage number popup (need to check if crit)
            numberPopupManager.spawnHitPopup(crit, healthDamage, this.transform, false);
            currentHealth -= healthDamage;
        }
    }
    //verify damage value etc
    [Server]
    public virtual void takeDamage(float damageValue, bool crit, float facing)
    {
        print("take damage command");
        //calculate armour value with armour pieces and buffs
        float armour = 0;
        if (enemyDetails is EnemyObjectScript)
        {
            armour = enemyDetails.armour;
            //calculate armour with buffs/debuffs
            foreach (ActiveBuffDebuff item in activeBuffsDebuffs)
            {
                BuffDebuff buff = item.buffDebuff;
                if (buff is BuffDebuff)
                {
                    //Get the buff info and apply it
                    if (buff.increaseorDecrease)
                    {
                        //increase the stat

                        if (buff.statToModify == BuffDebuff.stats.armour)
                        {
                            armour *= (1f + (buff.amount / 100f));
                        }
                    }
                    else
                    {
                        //Decrease the stat
                        if (buff.statToModify == BuffDebuff.stats.armour)
                        {
                            armour /= (1f + (buff.amount / 100f));
                        }
                    }
                }
            }

            //check if armour holds up against damage
            float healthDamage = damageValue - armour;
            RpctakeDamage(healthDamage, crit, facing);
        }
    }

    //do damage method implemented in subclasses

  

    //Collision detection and ground check, arrow check
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ring")
        {
            if (!collision.GetComponent<RingDetails>().playerSkill)
            {
                BuffDebuff buff = collision.GetComponent<RingDetails>().buff;
                this.addBuffDebuff(buff, collision.GetComponent<RingDetails>().sourceID);
            }

        }
    }







}
