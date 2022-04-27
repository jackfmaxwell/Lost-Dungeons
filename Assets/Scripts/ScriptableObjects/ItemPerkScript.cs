
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gear Talent", menuName = "Talent")]
public class ItemPerkScript : ScriptableObject
{

    //example: //- when hitting with the sword, 30% chance to transfer debuffs applied to you to the enemy youre hitting
    //condition - could be passive (always active) or conditional (on weapon attack, on critical hit ,...)
    //action:
    //increase a stat
    //heal hp (heal an ally, heal yourself, heal all)
    //modify buffs (remove buff, add buff, remove all buffs)

    //Note that there can be multiple conditions or actions
    [Header("Visual")]
    public string itemName;
    public string description;

    [Header("Logic")]
    public conditions[] condition;
    public enum conditions{ passive, onSwordHit, onBowHit, onCriticalHit, onDamageDone }
    

    public enum actions { increaseStat, healHP, removeBuff, addBuff}
    public actions[] action;

    //a talent has a condition and an action

    public BuffDebuffObjectScript[] buffs;
    public float[] buffDurations;
    public enum targets { player, enemy, allies, playerallies}
    public targets bufftarget;
}
