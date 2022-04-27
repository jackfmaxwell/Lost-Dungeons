
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Skill", menuName = "Skill")]
public class SkillObjectScript : ScriptableObject
{
    //example of a skill
    //C1 increase armour by X% for how much health you have for X seconds


    [Header("Visual")]
    public string skillName;
    public string skillDescription;
    public Sprite skillIcon;

    public int skillIndex; //for ui system, index list (saving system)

    [Header("Logic")]
    //object skills: instaniate an object like a ring of healing or ...
    //stat skills: modify a stat
    //buff skils: add/remove buffs/debuffs
    //action skills: do certain things like on an event like bow shot or taunt
    public skillTypes[] type;
    public enum skillTypes { statSkill, objectSkill, actionSkill, buffSkill }

    public BuffDebuffObjectScript[] skillBuffs;
    public float[] buffDuration; //1 to 1, index 0 skil buff has index 0 buff duration

    //Target for the skill
    public skillTargets[] target;
    public enum skillTargets { player, allies, playerAndAllies, nearbyEnemies }


    //stat skill
    [System.Serializable]
    public class affectedStatsClass{
        public affectedStats stat;
        public enum affectedStats { none, skillCooldowns, armour, health }

        public bool numberOrPercent; //true is number false is percent
        public float skillAmount; //to be treated as number increase if above is true, or percentage increase if above is false
    }
    public affectedStatsClass[] affectedStat;


    //object skill
    [Header("Object Type Class")]
    public objectTypeClass[] objectType;
    [System.Serializable]
    public class objectTypeClass
    {
        
        public objectType obj;
        public enum objectType { none, healingRing, shieldRing }

        public float duration;
    }
    


    //action skill
    public actionType[] action;
    public enum actionType { none, dash, taunt,  }

    

    [Header("Stats")]
    public float skillDuration;
    public float skillCooldown;
}
