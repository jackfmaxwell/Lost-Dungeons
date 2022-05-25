using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

//This class handles applying overtime buff effects every 1 second and manages adding and removing buffs
public class Player_Buffs : NetworkBehaviour
{
    
    //This class defines a structure that includes buffduff object script, current duration, total duration, and a constructor
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
    
    //Need a list of active buffs/debuffs
    [SerializeField]
    public List<ActiveBuffDebuff> activeBuffsDebuffs = new List<ActiveBuffDebuff>();

    //Functions to add and remove buffs
    public void addBuffDebuff(BuffDebuff buffDebuff, uint sourceID)
    {
        //is this buff already in the list?
        bool contained=false;
        
        foreach(ActiveBuffDebuff buff in activeBuffsDebuffs)
        {
            if(buff.id == sourceID) 
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

    //References
    private Player_Inventory inventory;
    private Player_Combat playerCombat;

    //get references
    private void Start()
    {
        try
        {
            playerCombat = this.gameObject.GetComponent<Player_Combat>();
            inventory = this.gameObject.GetComponent<Player_Inventory>();
        }
        catch (Exception e)
        {
            Debug.LogError("There was a problem finding the necessary dependents for Player Buffs");
            Debug.LogError(e);
        }

        InvokeRepeating("applyBuffDebuffEffect", 0f, 1f); //invoke applybuffeffect 
    }


    //called every 1 seconds, applies all buffs and counts down timers
    
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
                            print("healing from buff");
                            playerCombat.healPlayer(buff.amount); //CMD
                        }

                    }
                    else
                    {
                        //this buff increases the stat
                        if (buff.statToModify == BuffDebuff.stats.health)
                        {
                            playerCombat.applyHealthDamage(buff.amount); //CMD
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
        foreach (ActiveBuffDebuff item in toberemoved)
        {
            activeBuffsDebuffs.Remove(item);
        }
    }

    //this method calculates the armour with the buffs (that arent overtime)
    public float calculateArmourWithBuffs()
    {
        //get og armour
        float armour = inventory.getArmour();
        foreach (ActiveBuffDebuff item in activeBuffsDebuffs)
        {
            BuffDebuff buff = item.buffDebuff;
            if (buff is BuffDebuff)
            {
                if (!buff.overTime)
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
        }
        //calculte, then return the value
        return armour;
    }

    //calculate damage with buffs TODO 
    public float[] calculateDamageWithBuffs (int equippedWeaponIndex) //1 -> sword damage, 2-> bow damage, 3-> magic staff damage
    {
        //get og damage
        float totalDamage = 0;
        //calculate buffs
        float damage = 0;
        double critChance = 0, critDamage = 0;

        //Get initial details
        float[] charStats = new float[7];
        //what weapon do we have equipped

        charStats = inventory.getCharacterStats();
       
        damage = (float)charStats[equippedWeaponIndex];
        
        critChance = charStats[4];
        critDamage = charStats[5];
        
        
        //calculte, then return the value
        foreach (ActiveBuffDebuff item in activeBuffsDebuffs)
        {
            BuffDebuff buff = item.buffDebuff;
            if (buff is BuffDebuff)
            {
                //Get the buff info and apply it
                if (buff.increaseorDecrease)
                {
                    //this buff increases the stat
                    if (buff.statToModify == BuffDebuff.stats.critChance) //CC
                    {
                        //make sure critchance doesnt go more than 100%
                        if ((critChance * (1f + (buff.amount) / 100f)) > 100f)
                        {
                            critChance = 100f;
                        }
                        else
                        {
                            critChance *= (1f + (buff.amount) / 100f);
                        }

                    }
                    if (buff.statToModify == BuffDebuff.stats.critDamage) //CD
                    {
                        critDamage *= (1f + (buff.amount) / 100f);
                    }
                }
                else
                {
                    //Decrease the stat
                    if (buff.statToModify == BuffDebuff.stats.critChance)//CC
                    {
                        //Make sure crit chance doesnt go below 0
                        if ((critChance / (1f + (buff.amount / 100f))) > 0)
                        {
                            critChance /= (1f + (buff.amount) / 100f);
                        }
                        else
                        {
                            critChance = 0f;
                        }

                    }
                    if (buff.statToModify == BuffDebuff.stats.critDamage)//CD
                    {
                        critDamage /= (1f + (buff.amount) / 100f);
                    }
                }
            }


        }
        
        float[] updatedStats = { damage, (float)critChance, (float)critDamage };
        return updatedStats;
    }
}
