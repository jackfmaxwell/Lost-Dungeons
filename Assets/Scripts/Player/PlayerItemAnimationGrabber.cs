using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This class gets the animations from the items (helmet chest boot weapon and then adds the animations to the correct animators
//Used when different items have different animations (ex: helmet A and helmet B techincally have different animations since they
//look different
public class PlayerItemAnimationGrabber : MonoBehaviour
{
    public Animator helmetAnim, chestplateAnim, bootAnim, weaponAnim;
    private Player_Inventory inv;

    private AnimationClip[] helmetClips, chestplateClips, bootClips, weaponClips;

    private AnimatorOverrideController helmetAOC, chestplateAOC, bootAOC, weaponAOC;

    void Start()
    {
        updateAnimations();

    }
    private void Awake()
    {
        try {
            inv = this.GetComponent<Player_Inventory>();
        }
        catch(Exception e)
        {
            Debug.LogError("Playeritemanimation grabber had an issue finding inventory");
            Debug.LogError(e);
        }

    }


    public void updateAnimations()
    {
        //All the animations we need
        AnimationClip newHelmetIdle=null, newHelmetRun=null, newHelmetSwing=null, newHelmetBowShot=null;
        AnimationClip newChestplateIdle=null, newChestplateRun=null, newChestplateSwing=null, newChestplateBowShot = null;
        AnimationClip newBootIdle=null, newBootRun=null, newBootSwing=null, newBootBowShot = null;
        AnimationClip newWeaponIdle = null, newWeaponRun = null, newWeaponSwing = null, newWeaponBowShot = null, newWeaponBlock = null, newWeaponStaffAttack=null;

        //----------------------------------------------------------------------------------------------------------


        //Get all the animations from the equipped gear
        ItemObjectScript helmetObject = inv.equippedGear[0];
        if(helmetObject is ItemObjectScript)
        {
            newHelmetIdle = helmetObject.itemIdle;
            newHelmetRun = helmetObject.itemRun;
            newHelmetSwing = helmetObject.itemSwing;
            newHelmetBowShot = helmetObject.itemBowShot;
        }
       
        ItemObjectScript chestplateObject = inv.equippedGear[1];
        if(chestplateObject is ItemObjectScript)
        {
            newChestplateIdle = chestplateObject.itemIdle;
            newChestplateRun = chestplateObject.itemRun;
            newChestplateSwing = chestplateObject.itemSwing;
            newChestplateBowShot = chestplateObject.itemBowShot;
        }

        ItemObjectScript bootObject = inv.equippedGear[2];
        if (bootObject is ItemObjectScript)
        {
            newBootIdle = bootObject.itemIdle;
            newBootRun = bootObject.itemRun;
            newBootSwing = bootObject.itemSwing;
            newBootBowShot = bootObject.itemBowShot;
        }

        ItemObjectScript weapon1 = inv.equippedGear[3];
        if (weapon1 is ItemObjectScript)
        {
            if(weapon1.itemClass == ItemObjectScript.gearType.sword)
            {
                newWeaponIdle = weapon1.itemIdle;
                newWeaponRun = weapon1.itemRun;
                newWeaponSwing = weapon1.itemSwing;
            }
            if(weapon1.itemClass == ItemObjectScript.gearType.bow)
            {
                newWeaponBowShot = weapon1.itemBowShot;
            }
            if (weapon1.itemClass == ItemObjectScript.gearType.shield)
            {
                newWeaponBlock = weapon1.itemBlock;
            }
            if(weapon1.itemClass == ItemObjectScript.gearType.magicstaff)
            {
                newWeaponStaffAttack = weapon1.itemStaffAttack;
            }


        }

        ItemObjectScript weapon2 = inv.equippedGear[4];
        if(weapon2 is ItemObjectScript)
        {
            if (weapon2.itemClass == ItemObjectScript.gearType.sword)
            {
                newWeaponIdle = weapon2.itemIdle;
                newWeaponRun = weapon2.itemRun;
                newWeaponSwing = weapon2.itemSwing;
            }
            if (weapon2.itemClass == ItemObjectScript.gearType.bow)
            {
                newWeaponBowShot = weapon2.itemBowShot;
            }
            if (weapon2.itemClass == ItemObjectScript.gearType.shield)
            {
                newWeaponBlock = weapon2.itemBlock;
            }
            if (weapon2.itemClass == ItemObjectScript.gearType.magicstaff)
            {
                newWeaponStaffAttack = weapon2.itemStaffAttack;
            }
        }

        //Grabbed all the animations :)

      
        //Create the Animator Override Controllers, we use these to change the clips and the set it again
        helmetAOC = new AnimatorOverrideController(helmetAnim.runtimeAnimatorController);
        chestplateAOC = new AnimatorOverrideController(chestplateAnim.runtimeAnimatorController);
        bootAOC = new AnimatorOverrideController(bootAnim.runtimeAnimatorController);
        weaponAOC = new AnimatorOverrideController(weaponAnim.runtimeAnimatorController);
      
        //--------------------------------------------------------------------------------------
        
        helmetClips = helmetAOC.animationClips;
        //[0] idle
        //[1] run
        //[2] swing
        //[3] bowshot
        //Set the Override controller clips to the new clips
        helmetAOC[helmetClips[0]] = newHelmetIdle;
        helmetAOC[helmetClips[1]] = newHelmetRun;
        helmetAOC[helmetClips[2]] = newHelmetSwing;
        helmetAOC[helmetClips[3]] = newHelmetBowShot;
       
        //set the runtime controller to our override version
        helmetAnim.runtimeAnimatorController = helmetAOC;
        //------------------------------------------------------------  


        chestplateClips = chestplateAOC.animationClips;
        //[0] idle
        //[1] run
        //[2] swing
        //[3] bowshot
        chestplateAOC[chestplateClips[0]] = newChestplateIdle;
        chestplateAOC[chestplateClips[1]] = newChestplateRun;
        chestplateAOC[chestplateClips[2]] = newChestplateSwing;
        chestplateAOC[chestplateClips[3]] = newChestplateBowShot;

        //set the runtime controller to our override version
        chestplateAnim.runtimeAnimatorController = chestplateAOC;
        //------------------------------------------------------------


        bootClips = bootAOC.animationClips;
        //[0] bow shot
        //[1] idle
        //[2] run
        //[3] swing
        bootAOC[bootClips[0]] = newBootBowShot;
        bootAOC[bootClips[1]] = newBootIdle;
        bootAOC[bootClips[2]] = newBootRun;
        bootAOC[bootClips[3]] = newBootSwing;

        //set the runtime controller to our override version
        bootAnim.runtimeAnimatorController = bootAOC;
        //------------------------------------------------------------


        weaponClips = weaponAOC.animationClips;
        //[0] bow shot
        //[1] swing
        //[2] idle
        //[3] run
        weaponAOC[weaponClips[0]] = newWeaponBowShot;
        weaponAOC[weaponClips[1]] = newWeaponSwing;
        weaponAOC[weaponClips[2]] = newWeaponIdle;
        weaponAOC[weaponClips[3]] = newWeaponRun;
        weaponAOC[weaponClips[4]] = newWeaponBlock;
        weaponAOC[weaponClips[5]] = newWeaponStaffAttack;

        //set the runtime controller to our override version
        weaponAnim.runtimeAnimatorController = weaponAOC;
        //------------------------------------------------------------



    }


}
