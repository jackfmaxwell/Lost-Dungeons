using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Outline", menuName = "Enemy")]
public class EnemyObjectScript : ScriptableObject
{
    public float health;

    public AnimationClip idle, run, attack, bowShot;
    //take damage animation??

    public float armour;
    public float damage;

    //can enemies crit?
    public double critChance, critDamage;
    

    //Should the enemy be able to use a item database weapon? Dropped on death
    //  Maybe only high class enemies (grunts use non droppable weapons)

    ItemObjectScript equippedWeapon; //will be null if not using database item
    //if not null grab the animations
   


}
