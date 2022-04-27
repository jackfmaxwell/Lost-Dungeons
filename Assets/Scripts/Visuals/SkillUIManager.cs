using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    //Refrences
    [Header("Refrences")]
    public InventoryUIManager invUI; //to get currentPanel
    public Player_Inventory inv; //has the current equipped skills (scriptable)

    //The highlight section
    public Image skillIcon;
    public TMPro.TextMeshProUGUI skillTitle;
    public TMPro.TextMeshProUGUI skillDescription;
    public TMPro.TextMeshProUGUI skillCooldown;
    public TMPro.TextMeshProUGUI skillRequirement;
    public int[] skillSlotIndexs = new int[30]; //The indexs of each skill in slot
    public int highlightedSlot;
    public int currentSkillIndex;

    //bottom tooltip, when selecting a skill
    public TMPro.TextMeshProUGUI selectSlotTip;
    public TMPro.TextMeshProUGUI sameSkillTip;

    //equipped skills
    public Image skill1Image;
    public Image skill2Image;

    //skill slots
    public Image[] skillSlots;

    //skills can be locked
    //unlock skill with token, given when level up

    void Start()
    {
        updateSkillSlots();
        updateSelectedSkillSlots();
    }

    private int checkCurrentViewSlot(int firstNumberPos, string name)
    {
        int firstNumber = (int)char.GetNumericValue(name[firstNumberPos]);
        if (name[firstNumberPos + 1] == ')')
        {
            //thats the end of the numbr (1 dig)
            return firstNumber;
        }
        else
        {
            //not the end of the number
            int secondNumber = (int)char.GetNumericValue(name[firstNumberPos + 1]);
            string combinedNumberString = firstNumber.ToString() + secondNumber.ToString();

            int finalNumber = int.Parse(combinedNumberString);
            return finalNumber;
        }
    }
    void FixedUpdate()
    {
        if (invUI.currentPanel == InventoryUIManager.panels.Skills)
        {
            //if we are in the skills panel, call our functions here
            GameObject highlightObj = invUI.es.currentSelectedGameObject;
            string name = highlightObj.name;
            if (name[0] == 'S' && name[1]=='k' && name[2]=='i' && name[3]=='l' && name[4]=='l' && name[5]=='2')//6 is _ 7 is (
            {
                int finalNumber = checkCurrentViewSlot(8, name);

                highlightedSlot = finalNumber;

                currentSkillIndex = skillSlotIndexs[highlightedSlot];
                skillHighlightTip(currentSkillIndex);

            }
            //selected skills
            if(name[0]=='S' && name[1] == 'S')
            {
                //its skill 1
                if (name[6] == '1')
                {
                    SkillObjectScript theSkill = inv.skills[0];
                    if(theSkill is SkillObjectScript)
                    {
                        //display its info
                        skillHighlightTip(theSkill.skillIndex);
                    }
                    else
                    {
                        skillHighlightTip(0);
                    }
                }
                //its skill 2
                if (name[6] == '2')
                {
                    SkillObjectScript theSkill = inv.skills[1];
                    if (theSkill is SkillObjectScript)
                    {
                        //display its info
                        skillHighlightTip(theSkill.skillIndex);
                    }
                    else
                    {
                        skillHighlightTip(0);
                    }
                }
              
                highlightedSlot = -1; //Do this so we can verify we are looking at a valid slot
            }

            //Things we need to do:
            //Detect which skill is being highlight
            //Make sure its a skill we have credentials for (level, enough skill damage ect)
            //Grab the skill from database via position in index array 
            //Display skill info to highlight box
            //Cooldown and %s changed by skill haste and skill damage
            //If button press then equip it
            //show tooltip
            //wait for selected skill button press
            //move skill to that spot in skills[] (inv)
            //update the images

        }
    }

    private void skillHighlightTip(int index)
    {
        SkillObjectScript theSkill = inv.db.findSkillByIndex(index);
        if(theSkill is SkillObjectScript)
        {
            skillIcon.sprite = theSkill.skillIcon;
            skillTitle.text = theSkill.skillName;
            skillDescription.text = theSkill.skillDescription;
            skillCooldown.text = "[Base] " + theSkill.skillCooldown;
            skillRequirement.text = "Requires: NOT COMPLETE";
        }
        if (theSkill == null)
        {
            skillIcon.sprite = null;
            skillTitle.text = "";
            skillDescription.text = "";
            skillCooldown.text = "";
            skillRequirement.text = "";
        }
    }

    private void updateSkillSlots()
    {
        for(int i =0; i <30; i++)
        {
            SkillObjectScript theSkill = inv.db.findSkillByIndex(skillSlotIndexs[i]);
            if (theSkill != null)
            {
                if (theSkill is SkillObjectScript)
                {
                    skillSlots[i].sprite = theSkill.skillIcon;
                }
            }
        }

  
    }

    private void updateSelectedSkillSlots()
    {
        SkillObjectScript theLeftSkill = inv.skills[0];
        SkillObjectScript theRightSkill = inv.skills[1];
        if(theLeftSkill is SkillObjectScript)
        {
            skill1Image.sprite = theLeftSkill.skillIcon;
        }
        if(theRightSkill is SkillObjectScript)
        {
            skill2Image.sprite = theRightSkill.skillIcon;
        }

        
    }

    public bool selectingASkill = false;
    SkillObjectScript pickedSkill;

    //Need to make it so cant have two of the same skill
    //Button press on a skill to pick it
    public void selectSkill(int slot) //param is -1 for left and 1 for right, 0 if its a skill list slot
    {
        sameSkillTip.gameObject.SetActive(false);
        
        //need to figure out how to check the slot we are looking at is not empty
        if (selectingASkill == false && slot == 0 && currentSkillIndex != 0)
        {
            selectingASkill = true;
            //now check for its slot to be picked
            selectSlotTip.gameObject.SetActive(true);

            //save the skill in mem so we can add later
            pickedSkill = inv.db.findSkillByIndex(currentSkillIndex);
        }
        else
        {
            //we are picking the slot
            //double check that we are clicking a valid slot not back in skill pool
            if (highlightedSlot == -1)
            {
                if(pickedSkill==inv.skills[0] || pickedSkill == inv.skills[1])
                {
                    sameSkillTip.gameObject.SetActive(true); //need to turn this off after couple seconds (coroutine)
                }
                else
                {
                    //yup lets go ahead and select this
                    if (pickedSkill is SkillObjectScript)
                    {
                        //lets go ahead and add to our equipped then
                        if (slot == -1)
                        {
                            inv.skills[0] = pickedSkill;
                            updateSelectedSkillSlots();

                        }
                        if (slot == 1)
                        {
                            inv.skills[1] = pickedSkill;
                            updateSelectedSkillSlots();
                        }
                    }
                }

            }

            selectingASkill = false;
            selectSlotTip.gameObject.SetActive(false);
        }






    }
}
