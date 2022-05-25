
//THIS CLASS IS USED TO HOLD THE RUNTIME INFORMATION FOR BUFFS
//BUFFDEBUFFOBJECTSCRIPT HOLDS THE DEFAULT VALUES THAT WE APPLY OUR STATS ON
//WE THEN WRITE THESE UPDATED VALUES TO THIS CLASS WHICH IS USED TO SEND ACROSS NETWORK AND 
//APPLY TO THE CHARACTERS
public class BuffDebuff
{
    //CLASSIC BUFF FRAMEWORK
    public string buffName; //must be unique for sprite lookup to work

    //outline for the buff/debuff
    public bool increaseorDecrease; //true means increase, false means decrease 
    public enum stats { armour, critChance, critDamage, skillDamage, skillHaste, health }
    public stats statToModify;

    //A percentage increase
    public float amount;

    public bool overTime; //buff effect is an overtime 
    //----------------------------

    public float buffDuration;

    public BuffDebuff()
    {

    }
    public BuffDebuff(string name, bool iOrd, stats stat, float amt, bool overT, float duration)
    {
        buffName = name;
        increaseorDecrease = iOrd;
        statToModify = stat;
        amount = amt;
        overTime = overT;
        buffDuration = duration;
    }
}
