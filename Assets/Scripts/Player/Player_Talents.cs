using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//this class listens to talent events and applies the logic
public class Player_Talents : MonoBehaviour
{
    public uint sourceID;
    private Player_Inventory inventory;
    private void Awake()
    {
        sourceID = (uint)UnityEngine.Random.Range(0, uint.MaxValue);
        try
        {
            inventory = this.GetComponent<Player_Inventory>();
        }
        catch(Exception e)
        {
            Debug.LogError("Player Talents class could not find required refrences.");
            Debug.LogError(e);
        }
       
    }
    public void arrowHitTargetEvent(GameObject gameobject)
    {
        
        ItemObjectScript helmet = inventory.equippedGear[0];
        ItemObjectScript chestplate = inventory.equippedGear[1];
        ItemObjectScript boots = inventory.equippedGear[2];
        ItemObjectScript sword = inventory.equippedGear[3];
        ItemObjectScript bow = inventory.equippedGear[4];
        ItemObjectScript magicstaff = inventory.equippedGear[5];
        ItemObjectScript shield = inventory.equippedGear[6];

        foreach(ItemPerkScript talent in bow.talents)
        {
            if (talent.condition[0] == ItemPerkScript.conditions.onBowHit)
            {
                if (talent.action[0] == ItemPerkScript.actions.addBuff)
                {
                    if (talent.bufftarget == ItemPerkScript.targets.enemy)
                    {
                        BuffDebuff buffInstance = new BuffDebuff(talent.buffs[0].buffName, talent.buffs[0].increaseorDecrease, (BuffDebuff.stats)talent.buffs[0].statToModify, talent.buffs[0].amount, talent.buffs[0].overTime, talent.buffDurations[0]);
                        gameobject.GetComponent<Enemy_Generic>().addBuffDebuff(buffInstance, sourceID);
                        
                    }
                }
            }
        }
    }
}
