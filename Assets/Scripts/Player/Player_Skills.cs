using UnityEngine;
using System;
using Mirror;

//This class handles implementing the skill timers
//This class also handles the skill logic, this means decoding the skill scriptable object into actions
public class Player_Skills : NetworkBehaviour
{

    //Skill cooldown and duration timers for 1 and 2
    public bool skill1Started;
    public bool skill1Ready;
    public float skill1Duration, skill1Cooldown;
    public float skill1TotalDur, skill1TotalCool; //refernced somewhere else

    public bool skill2Started;
    public bool skill2Ready;
    public float skill2Duration, skill2Cooldown;
    public float skill2TotalDur, skill2TotalCool; //refernced somewhere else

    //Physical skill objects
    public GameObject healingRing;
    public GameObject shieldRing;

    //References
    private Player_Inventory stats; //grabs the player stats
    private Player_Buffs playerBuffs;
    //Need refrence to player to apply skill logic
    private Player_Combat playerCombat;//can talk to player_combat


    //This method handles the skill cooldown as well as the skill duration for skill 1
    private void skill1CooldownLogic()
    {
        //skill duration
        if (skill1Started)
        {
            if (skill1Duration > 0f)
            {
                skill1Duration -= 1f * Time.deltaTime;
            }
            else
            {
                //Skill duration over
                skill1Started = false;
            }
        }
        //skill cooldown
        if (skill1Started == false && skill1Ready != true)
        {
            if (skill1Cooldown > 0f)
            {
                skill1Cooldown -= 1f * Time.deltaTime;
            }
            else
            {
                skill1Ready = true;
            }
        }
    }
    //This method handles the skill cooldown as well as the skill duration for skill 2
    private void skill2CooldownLogic()
    {
        if (skill2Started)
        {
            if (skill2Duration > 0f)
            {
                skill2Duration -= 1f * Time.deltaTime;
            }
            else
            {
                //Skill duration over
                skill2Started = false;
            }
        }
        if (skill2Started == false && skill2Ready != true)
        {
            if (skill2Cooldown > 0f)
            {
                skill2Cooldown -= 1f * Time.deltaTime;
            }
            else
            {
                skill2Ready = true;
            }
        }
    }



    private void Awake()
    {
        try
        {
            playerBuffs = this.GetComponent<Player_Buffs>();
            stats = this.GetComponent<Player_Inventory>();
            playerCombat = this.GetComponent<Player_Combat>(); //grab playerCombat class
        }
        catch(Exception e)
        {
            Debug.LogError("There was a problem finding the necessary dependents for Player Animation.");
            Debug.LogError(e);
        }
        
    }


    private void Update()
    {
        skill1CooldownLogic();
        skill2CooldownLogic();
    }

    //Set the stats for the skill duration and cooldown based on skill stats in player
    private void startSkill1(float duration, float cooldown)
    {
        float[] stat = stats.getCharacterStats();

        skill1Started = true;
        skill1Ready = false;
        skill1Duration = duration;
        skill1Cooldown = cooldown - (cooldown * stat[6]);

        skill1TotalDur = duration; 
        skill1TotalCool = cooldown - (cooldown * stat[6]);
    }
    [Command]
    void CmdSpawnHealingRing(float objectDuration, BuffDebuff buffInstance)
    {
        GameObject ring = GameObject.Instantiate(healingRing);
        ring.transform.position = this.gameObject.transform.position;

        ring.GetComponent<RingDetails>().playerSkill = true;
        ring.GetComponent<RingDetails>().duration = buffInstance.buffDuration;
        //COULD BE ISSUE HERE TODO
        ring.GetComponent<RingDetails>().buff = buffInstance; //this may not work because sprite reference in BuffObjectScript
        ring.GetComponent<RingDetails>().sourceID = (uint)UnityEngine.Random.Range(0, uint.MaxValue);
        
        NetworkServer.Spawn(ring);
    }

    //This method handles logic for skill1
    public void useSkill1()
    {
        //Grab skill details from scriptableobject

        SkillObjectScript skill1 = stats.skills[0];
        uint sourceID = (uint)UnityEngine.Random.Range(0, uint.MaxValue);

        //check if there is a skill we can use
        if (skill1 is SkillObjectScript)
        {
            //is skill ready?
            if (skill1Ready)
            {
                float[] stat = stats.getCharacterStats();
                startSkill1(skill1.skillDuration, skill1.skillCooldown); //sets the cooldown and duration 

                //Do skill logic
                foreach (SkillObjectScript.skillTypes type in skill1.type)
                {
                    //creates an object
                    if (type == SkillObjectScript.skillTypes.objectSkill)
                    {
                        //need to spawn on object
                        foreach (SkillObjectScript.objectTypeClass objectTypeClass in skill1.objectType)
                        {

                            if (objectTypeClass.obj == SkillObjectScript.objectTypeClass.objectType.healingRing)
                            {
                                //instantiate object
                                BuffDebuffObjectScript skillBuff = skill1.skillBuffs[0];
                                BuffDebuff buffWithStatsApplied = new BuffDebuff(skillBuff.buffName, skillBuff.increaseorDecrease, (BuffDebuff.stats)skillBuff.statToModify, skillBuff.amount, skillBuff.overTime, skill1.buffDuration[0]);
                                CmdSpawnHealingRing(objectTypeClass.duration, buffWithStatsApplied);

                            }
                            if (objectTypeClass.obj == SkillObjectScript.objectTypeClass.objectType.shieldRing)
                            {
                                //CmdSpawnSkillObject(ring);
                            }

                        }
                    }

                    //add a buff
                    if (type == SkillObjectScript.skillTypes.buffSkill)
                    {
                        //need to add a buff
                        int i = 0;
                        foreach (BuffDebuffObjectScript buff in skill1.skillBuffs)
                        {
                            BuffDebuff buffInstance = new BuffDebuff(buff.buffName, buff.increaseorDecrease, (BuffDebuff.stats)buff.statToModify, buff.amount, buff.overTime, skill1.buffDuration[i]);
                            playerBuffs.addBuffDebuff(buffInstance, sourceID);
                            print("added");
                            i++;
                        }
                    }

                    //modify stat
                    if (type == SkillObjectScript.skillTypes.statSkill)
                    {
                        //Loop through the affectedStats of the skill
                        foreach (SkillObjectScript.affectedStatsClass affectedStatClass in skill1.affectedStat)
                        {
                            //modifies skill cooldowns
                            if (affectedStatClass.stat == SkillObjectScript.affectedStatsClass.affectedStats.skillCooldowns)
                            {
                                foreach (SkillObjectScript.skillTargets skillTarget in skill1.target)
                                {
                                    if (skillTarget == SkillObjectScript.skillTargets.player)
                                    {
                                        //player
                                        skill2Cooldown -= affectedStatClass.skillAmount * (1f + (stat[6] / 100f));
                                        if (skill2Cooldown < 0f)
                                        {
                                            skill2Cooldown = 0f;
                                        }
                                    }
                                    if (skillTarget == SkillObjectScript.skillTargets.playerAndAllies)
                                    {
                                        //player
                                        skill2Cooldown -= affectedStatClass.skillAmount * (1f + (stat[6] / 100f));
                                        if (skill2Cooldown < 0f)
                                        {
                                            skill2Cooldown = 0f;
                                        }

                                        //and allies?
                                        //probably have a variable refrencing the other player classes in your game
                                    }
                                }

                            }

                            //modifies health
                            if (affectedStatClass.stat == SkillObjectScript.affectedStatsClass.affectedStats.health)
                            {
                                foreach (SkillObjectScript.skillTargets skillTarget in skill1.target)
                                {
                                    if (skillTarget == SkillObjectScript.skillTargets.player)
                                    {
                                        //player
                                        playerCombat.healPlayer(affectedStatClass.skillAmount * (1f + (stat[6] / 100f)));
                                    }
                                    if (skillTarget == SkillObjectScript.skillTargets.playerAndAllies)
                                    {
                                        //player
                                        playerCombat.healPlayer(affectedStatClass.skillAmount * (1f + (stat[6] / 100f)));

                                        //and allies?
                                        //probably have a variable refrencing the other player classes in your game
                                    }
                                }
                            }
                        }



                    }
                }

            }

        }
    }

    

    private void startSkill2(float duration, float cooldown)
    {
        float[] stat = stats.getCharacterStats();

        skill2Started = true;
        skill2Ready = false;
        skill2Duration = duration;
        skill2Cooldown = cooldown - (cooldown * stat[6]);

        skill2TotalDur = duration;
        skill2TotalCool = cooldown - (cooldown * stat[6]);
    }
    public void useSkill2()
    {
        uint sourceID = (uint)UnityEngine.Random.Range(0, uint.MaxValue);

        SkillObjectScript skill2 = stats.skills[1];
        //check if there is a skill we can use
        if (skill2 is SkillObjectScript)
        {
            if (skill2Ready)
            {
                float[] stat = stats.getCharacterStats();
                startSkill2(skill2.skillDuration, skill2.skillCooldown);

                //Do skill logic

                foreach (SkillObjectScript.skillTypes type in skill2.type)
                {
                    if (type == SkillObjectScript.skillTypes.objectSkill)
                    {
                        //need to spawn on object
                        foreach (SkillObjectScript.objectTypeClass objectTypeClass in skill2.objectType)
                        {

                            if (objectTypeClass.obj == SkillObjectScript.objectTypeClass.objectType.healingRing)
                            {
                                //instantiate object


                            }
                            if(objectTypeClass.obj == SkillObjectScript.objectTypeClass.objectType.shieldRing)
                            {

                            }

                        }
                    }

                    if (type == SkillObjectScript.skillTypes.buffSkill)
                    {
                        //need to add a buff
                        int i = 0;
                        foreach (BuffDebuffObjectScript buff in skill2.skillBuffs)
                        {
                            BuffDebuff buffInstance = new BuffDebuff(buff.buffName, buff.increaseorDecrease, (BuffDebuff.stats)buff.statToModify, buff.amount * stat[6], buff.overTime, skill2.buffDuration[i]);
                            playerBuffs.addBuffDebuff(buffInstance, sourceID);
                            
                            i++;
                        }
                    }

                    if (type == SkillObjectScript.skillTypes.statSkill)
                    {
                       
                        //Loop through the affectedStats of the stat skill class
                        foreach (SkillObjectScript.affectedStatsClass affectedStatClass in skill2.affectedStat)
                        {
                            //cooldowns
                            if (affectedStatClass.stat == SkillObjectScript.affectedStatsClass.affectedStats.skillCooldowns)
                            {
                                foreach (SkillObjectScript.skillTargets skillTarget in skill2.target)
                                {
                                    if (skillTarget == SkillObjectScript.skillTargets.player)
                                    {
                                        //player
                                        skill2Cooldown -= affectedStatClass.skillAmount * (1f + (stat[6] / 100f));
                                        if (skill2Cooldown < 0f)
                                        {
                                            skill2Cooldown = 0f;
                                        }
                                    }
                                    if (skillTarget == SkillObjectScript.skillTargets.playerAndAllies)
                                    {
                                        //player
                                        skill2Cooldown -= affectedStatClass.skillAmount * (1f + (stat[6] / 100f));
                                        if (skill2Cooldown < 0f)
                                        {
                                            skill2Cooldown = 0f;
                                        }

                                        //and allies?
                                        //probably have a variable refrencing the other player classes in your game
                                    }
                                }

                            }

                            if (affectedStatClass.stat == SkillObjectScript.affectedStatsClass.affectedStats.health)
                            {
                                foreach (SkillObjectScript.skillTargets skillTarget in skill2.target)
                                {
                                    if (skillTarget == SkillObjectScript.skillTargets.player)
                                    {
                                        //player
                                        playerCombat.healPlayer(affectedStatClass.skillAmount * (1f + (stat[6] / 100f)));
                                    }
                                    if (skillTarget == SkillObjectScript.skillTargets.playerAndAllies)
                                    {
                                        //player
                                        playerCombat.healPlayer(affectedStatClass.skillAmount * (1f + (stat[6] / 100f)));

                                        //and allies?
                                        //probably have a variable refrencing the other player classes in your game
                                    }
                                }
                            }
                        }



                    }
                }

            }

        }
    }

   
    
}
