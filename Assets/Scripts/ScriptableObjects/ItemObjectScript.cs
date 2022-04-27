
using System;
using UnityEngine;

[CreateAssetMenu(fileName ="Gear Item", menuName = "Item")]
public class ItemObjectScript : ScriptableObject
{
    [Header("Visual")]
    public Sprite itemPicture;

    public string itemName;
    public int itemIndex; //to save and load
    public string itemDescription;
    public enum gearRarity { common, uncommon, rare, legendary, exotic, artifact, energized}
    public gearRarity itemRarity;
    public enum gearType { helmet, chestplate, boot, sword, bow, magicstaff, shield }
    public gearType itemClass;

    public AnimationClip itemIdle, itemRun, itemSwing, itemBowShot, itemBlock, itemStaffAttack; //if item is a sword, bowShot animation will not be needed, if item is a bow, swing animation will not be needed

    [Header("Stats")]
    public float weaponDamage; //will be 0 if armouor piece
    public float armour; //will be 0 if weapon

    public double critChance, critDamage; //both need to spawn in an range
    public double skillDamage, skillHaste; //both need to spawn in an range

    public ItemPerkScript[] talents; //either a weapon talent or armour talent

    //Perks
    //need scriptable object for this

    //general idea

        //player skills (pick 2)
            //-healing
                //A1 reduce your and allies cooldowns by X seconds and give allies 30% of HP back (requries certain amount of skill damage to work)
                //A2 increase you and allies armour by X% for X seconds and heal X% hp back per second
                //A3 place an area heal effect that heals allies X% hp back for X seconds and removes any negative effects (requries high skill damage)
                //A4 buff nearby ally skill damage by X%, crit damage by X%, and crit chance by X% for X seconds
            //-damage
                //B1 increases your crit chance by 40% (X%) and crit damage by 100% (X%) for the next 5 attacks
                //B2 reduce the armour of enemies in a area around a landed bow shot by 60% (X%) for 10 seconds (X seconds)
                //B3 for X seconds you have 100% crit chance and 200% crit damage, and your weapon damage is increased by X%
                //B4 quickly dash in a direction, enemies lose focus on you, regenerate 30% of your health (must have certain low amount of skill damage/timing)
            //-tank
                //C1 increase armour by X% for how much health you have for X seconds
                //C2 place a shield that has X amount of HP (increases by players armour or health) and protects those inside, lasts for X seconds (must have certain low amount of skill timing and high armour)
                //C3 taunt enemies causing them to focus on you, also weakens enemies that shoot at you damage and armour by X%
                //C4 increase your armour by 50% (X%) for X seconds after X seconds return all damage * 1.5 to the first enemy you hit with a sword
     

        //weapon talents
            //-healing
                //- this weapon doesnt do damage and instead will heal allies when hitting them
                //- returns 50% of damage done back to you and your allies

            //-damage
                //- increases damage by X% for every X% of health missing
                //- critical hits with the bow increase skill haste by X% and give a buff to armour for X%

            //-tank
                //- hits with the bow give a buff that gives the amount of damage done to multiply armour amount, stacks up to 3 times
                //- when hitting with the sword, 30% chance to transfer debuffs applied to you to the enemy youre hitting

       //armour talents
            //-healing
            //-damage
            //-tank
                  //- blocked hits heal hp for the amount of dmg blocked



        //when an attack is hit on an entity
            //- first any buffs/debuffs are processed (could be increasing attack damage at random chance, reducing enemy armour, increasing your armour, ...)
            //- then check for critical hits, roll chance, then multiply weapon damage calculated from previous step with crit damage multiplier
            //- then take this damage number and subtract the targets armour from it to calculate health damage



    //sword
        //rarity
        //damage stat - (How much damage this weapon does)
        //crit chance - (% chance, rolled every attack)
        //crit damage - (% increase to the base damage when a critical hit is scored)
        //skill damage stat - (% multiplier to increase skill damage)
        //skill timing stat - (% multiplier to reduce skill cooldowns)
        //talent 1 

    //bow
        //rarity
        //damage stat - (How much damage this weapon does)
        //crit chance - (% chance, rolled every attack)
        //crit damage - (% increase to the base damage when a critical hit is scored)
        //skill damage stat - (% multiplier to increase skill damage)
        //skill timing stat - (% multiplier to reduce skill cooldowns)
        //talent 1 

    //helmet
        //rarity
        //armour stat - (How much damage this weapon does)
        //crit chance - (% chance, rolled every attack)
        //crit damage - (% increase to the base damage when a critical hit is scored)
        //skill damage stat - (% multiplier to increase skill damage)
        //skill timing stat - (% multiplier to reduce skill cooldowns)
        //talent 1 

    //chestplate
        //rarity
        //armour stat - (How much damage this weapon does)
        //crit chance - (% chance, rolled every attack)
        //crit damage - (% increase to the base damage when a critical hit is scored)
        //skill damage stat - (% multiplier to increase skill damage)
        //skill timing stat - (% multiplier to reduce skill cooldowns)
        //talent 1 

    //boot
        //rarity
        //armour stat - (How much damage this weapon does)
        //crit chance - (% chance, rolled every attack)
        //crit damage - (% increase to the base damage when a critical hit is scored)
        //skill damage stat - (% multiplier to increase skill damage)
        //skill timing stat - (% multiplier to reduce skill cooldowns)
        //talent 1 

    //shield
        //rarity
        //armour stat
        
        //talent 1
    
}
