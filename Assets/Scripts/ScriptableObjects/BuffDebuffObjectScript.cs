using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BuffDebuff Outline", menuName = "BuffDebuff")]
public class BuffDebuffObjectScript : ScriptableObject
{
    public Sprite image;
    public string buffName; //must be unique for sprite lookup to work

    //outline for the buff/debuff
    public bool increaseorDecrease; //true means increase, false means decrease 
    public enum stats { armour, critChance, critDamage, skillDamage, skillHaste, health}
    public stats statToModify;

    //A percentage increase
    [Tooltip("A percentage increase")]
    public float amount;

    public bool overTime; //buff effect is an overtime 

}
