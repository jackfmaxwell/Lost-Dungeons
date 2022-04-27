using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Com.LuisPedroFonseca.ProCamera2D;
using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//This class translates inventory code into visual images in a UI system. This class also contains the logic to allow
//the player to interface with this display and equip different items
public class InventoryUIManager : MonoBehaviour
{
    //Refrences
    [Header("Refrences")]
    public EventSystem es; //Ref to eventsystem to detect which item slot is selected
    public Player_Inventory inv; //Ref to inventory class which holds players items

    //-----------------------------------------------------------------------------------------------------------------------------------------

    //This method looks at ui pos.x and then decides which panel we are on
    [Header("UI Panels")]
    public panels currentPanel;
    public enum panels { Multiplayer, Inventory, Skills, Settings }
    private void detectCurrentPanel()
    {
        if (targetLoc ==0) //0
        {
            currentPanel = panels.Inventory;
        }
        if (targetLoc==-850) //-850
        {
            currentPanel = panels.Skills;
        }
        if (targetLoc==-1700) //-1700
        {
            currentPanel = panels.Settings;
        }
        if (targetLoc==850) //850
        {
            currentPanel = panels.Multiplayer;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------

    //Movement between panels
    [Header("Panel Movement")]
    float targetLoc = 0;
    public float movementRate = 50;
    public GameObject ui; //for panel control
    public Button inventoryFirstSelect, skillsFirstSelect;
    public bool movingL, movingR = false;
    private void panelSwitching()
    {
        //-----------------------------------------INPUT--------------------------------------//
        //get input
        float bumpersLeft = Input.GetAxis("Skill1");
        float bumpersRight = Input.GetAxis("Skill2");

        //need to smoothly transition UI 850 <-> 0 <-> -850 <-> -1700
        //if not current transitioning
        if (!movingL && !movingR)
        {
            //if bumper left and not on far left panel
            if (bumpersLeft > 0 && currentPanel != panels.Multiplayer)
            {
                float startLoc = ui.transform.localPosition.x;
                targetLoc = ui.transform.localPosition.x + 850;
                movingL = true;

              
            }
            //if bumper right and not on far right panel
            if (bumpersRight > 0 && currentPanel != panels.Settings)
            {
                float startLoc = ui.transform.localPosition.x;
                targetLoc = ui.transform.localPosition.x - 850;
                movingR = true;

                
            }
        }

        //-----------------------------------------MOVEMENT--------------------------------------//
        if (movingL)
        {
            //if weve reached the target location, stop moving
            if (ui.transform.localPosition.x >= targetLoc)
            {
                movingL = false;
                //select first item when switching panel
                if (currentPanel == panels.Inventory)
                {
                    inventoryFirstSelect.Select();
                    print("Inventory first select complete");
                }
                if (currentPanel == panels.Skills)
                {
                    skillsFirstSelect.Select();
                    print("Skills first select complete");
                }

            }
            //move ui position
            else
            {
                float distancePerFrame = movementRate * Time.deltaTime;
                ui.transform.localPosition += new Vector3(distancePerFrame, 0, 0);
            }
        }
        if (movingR)
        {
            //if weve reached the target location, stop moving
            if (ui.transform.localPosition.x <= targetLoc)
            {
                movingR = false;
                //select first item when switching panel
                if (currentPanel == panels.Inventory)
                {
                    inventoryFirstSelect.Select();
                    print("Inventory first select complete");
                }
                if (currentPanel == panels.Skills)
                {
                    skillsFirstSelect.Select();
                    print("Skills first select complete");
                }
            }
            //move ui position
            else
            {
                float distancePerFrame = movementRate * Time.deltaTime;
                ui.transform.localPosition -= new Vector3(distancePerFrame, 0, 0);
            }
        }

        //END OF FUNCTION
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------

    //logic to figure out which itemslot is being highlighted by the player
    [Header("Item Slot Highlight")]
    public int[] itemSlots = new int[63]; //The itemIndex of each item slot in inventory
    public int highlightSlot; //The currently highlighted slot
    public int currentHighlightItemIndex;
    public bool cantEquip=false;
    private void itemHighlight()
    {
        if (currentPanel == panels.Inventory)
        {
            GameObject highlightedObject = es.currentSelectedGameObject;

            if (highlightedObject != null)
            {
                string name = highlightedObject.name; //the highlighted object name
                //if the name starts with I (all the item slots, not equipped gear)
                if (name[0] == 'I')
                {
                    cantEquip = false;
                    int firstNumber = (int)char.GetNumericValue(name[7]); //grab the first number from name string
                                                                          //if only 1 digit
                    if (name[8] == ')')
                    {
                        highlightSlot = firstNumber; //set current highlightedSlot
                    }
                    //if more than 1 digit
                    else
                    {
                        int secondNumber = (int)char.GetNumericValue(name[8]);//grab the second number from name string
                        string combinedNumberString = firstNumber.ToString() + secondNumber.ToString(); //combine both grabbed numbers into one string

                        int number = int.Parse(combinedNumberString); //turn the combined string into an int
                        highlightSlot = number;//set current highlightedSlot

                    }
                    currentHighlightItemIndex = itemSlots[highlightSlot];
                }
                //Equipped Gear
                if (name[0] == 'E' && name[1]=='G')
                {
                    cantEquip = true;   
                    
                    //helmet
                    if (name == "EGHelmet")
                    {
                        if(inv.equippedGear[0] is ItemObjectScript)
                        {
                            currentHighlightItemIndex = inv.equippedGear[0].itemIndex;
                        }
                        
                    }
                    //chest
                    if (name == "EGChestplate")
                    {
                        if (inv.equippedGear[1] is ItemObjectScript)
                        {
                            currentHighlightItemIndex = inv.equippedGear[1].itemIndex;
                        }
                    }
                    //boot
                    if (name == "EGBoot")
                    {
                        if (inv.equippedGear[2] is ItemObjectScript)
                        {
                            currentHighlightItemIndex = inv.equippedGear[2].itemIndex;
                        }
                    }
                    //sword
                    if (name=="EGSword")
                    {
                        if (inv.equippedGear[3] is ItemObjectScript)
                        {
                            currentHighlightItemIndex = inv.equippedGear[3].itemIndex;
                        }
                    }
                    //bow
                    if (name=="EGBow")
                    {
                        if (inv.equippedGear[4] is ItemObjectScript)
                        {
                            currentHighlightItemIndex = inv.equippedGear[4].itemIndex;
                        }
                    }
                }

            }

            //the current highlighted item index is itemSlots[highlightSlot];
           
            itemHighlightTooltipUI(currentHighlightItemIndex); //should i call this on frame we select new one?

        }
        //END OF FUNCTION

    }


    public Image itemFrame;
    public Image itemRarity; //Colors //common, uncommon, rare, legendary, exotic, ancient, energized
    public Color common, uncommon, rare, legendary, exotic, artifact, energized, empty;
    public TMPro.TextMeshProUGUI itemName;
    public TMPro.TextMeshProUGUI itemDescription;
    public TMPro.TextMeshProUGUI itemDamage;
    public TMPro.TextMeshProUGUI itemCritChance;
    public TMPro.TextMeshProUGUI itemCritDamage;
    public TMPro.TextMeshProUGUI itemSkillDamage;
    public TMPro.TextMeshProUGUI itemSkillHaste;
    public TMPro.TextMeshProUGUI talent1Name;
    public TMPro.TextMeshProUGUI talent1Description;
    public TMPro.TextMeshProUGUI talent2Name;
    public TMPro.TextMeshProUGUI talent2Description;
    private void itemHighlightTooltipUI(int currentIndex)
    {
        //get the item
        ItemObjectScript theItem = inv.db.findItemByIndex(currentHighlightItemIndex);

        //we are looking at an empty slot
        if (theItem == null)
        {
            
            itemFrame.sprite = null;
            itemRarity.color = empty;
            itemName.text = "";
            itemDescription.text = "";
            itemDamage.text = "";
            itemCritChance.text = "";
            itemCritDamage.text = "";
            itemSkillDamage.text = "";
            itemSkillHaste.text = "";
            talent1Name.text = "";
            talent1Description.text = "";

            talent2Name.text = "";
            talent2Description.text = "";
        }
        if(theItem is ItemObjectScript)
        {
            //set UI
            itemFrame.sprite = theItem.itemPicture;
            if(theItem.itemRarity == ItemObjectScript.gearRarity.common)
            {
                itemRarity.color = common;
            }
            if (theItem.itemRarity == ItemObjectScript.gearRarity.uncommon)
            {
                itemRarity.color = uncommon;
            }
            if (theItem.itemRarity == ItemObjectScript.gearRarity.rare)
            {
                itemRarity.color = rare;
            }
            if (theItem.itemRarity == ItemObjectScript.gearRarity.legendary)
            {
                itemRarity.color = legendary;
            }
            if (theItem.itemRarity == ItemObjectScript.gearRarity.exotic)
            {
                itemRarity.color = exotic;
            }
            if (theItem.itemRarity == ItemObjectScript.gearRarity.artifact)
            {
                itemRarity.color = artifact;
            }
            if (theItem.itemRarity == ItemObjectScript.gearRarity.energized)
            {
                itemRarity.color = energized;
            }

            itemName.text = theItem.itemName;
            itemDescription.text = theItem.itemDescription;
            //weapon
            if (theItem.itemClass == ItemObjectScript.gearType.sword || theItem.itemClass == ItemObjectScript.gearType.bow)
            {
                itemDamage.text = "Damage: " + theItem.weaponDamage;
            }
            //armour
            else
            {
                itemDamage.text = "Armour: " + theItem.armour;
            }
            itemCritChance.text = "Critical Chance: " + theItem.critChance;
            itemCritDamage.text = "Critical Damage: " + theItem.critDamage;
            itemSkillDamage.text = "Skill Damage: " + theItem.skillDamage;
            itemSkillHaste.text = "Skill Haste: " + theItem.skillHaste;


            if (theItem.talents.Length == 0)
            {
                //empty texts
                talent1Name.text = "";
                talent1Description.text = "";

                talent2Name.text = "";
                talent2Description.text = "";
            }
            if (theItem.talents.Length==1)
            {
                ItemPerkScript talent1 = theItem.talents[0];
                if (talent1 is ItemPerkScript)
                {
                    talent1Name.text = theItem.talents[0].itemName;
                    talent1Description.text = theItem.talents[0].description;
                }
                talent2Name.text = "";
                talent2Description.text = "";
            }
            if (theItem.talents.Length == 2)
            {
                ItemPerkScript talent1 = theItem.talents[0];
                if (talent1 is ItemPerkScript)
                {
                    talent1Name.text = theItem.talents[0].itemName;
                    talent1Description.text = theItem.talents[0].description;
                }
                ItemPerkScript talent2 = theItem.talents[1];
                if (talent2 is ItemPerkScript)
                {
                    talent2Name.text = theItem.talents[1].itemName;
                    talent2Description.text = theItem.talents[1].description;
                }
            }

        }
    }

    public int LRUequippedweaponindex = 3;
    public void equipHighlightedItem() //public method thats called when selected item slot
    {
        ItemObjectScript theItem = inv.db.findItemByIndex(currentHighlightItemIndex);
        //this item should be in our iventory since it is projected into ui from inventory
        if (theItem is ItemObjectScript && !cantEquip)
        {
            //get the objects class
            ItemObjectScript.gearType type = theItem.itemClass;

            //which slot does it go to?
            if (type == ItemObjectScript.gearType.helmet)
            {
                //get the gear item we are replacing
                ItemObjectScript replacedGear = inv.equippedGear[0];

                inv.equippedGear[0] = theItem;
                updateGearHotBar(); //update the equipped gear frame to show new gear

                //now put the replaced gear into the inventory and remove old item from inventory
                inv.removeItemFromInventory(currentHighlightItemIndex);
                inv.addItemToInventory(replacedGear.itemIndex);
                
            }
            if (type == ItemObjectScript.gearType.chestplate)
            {
                //get the gear item we are replacing
                ItemObjectScript replacedGear = inv.equippedGear[1];
                

                inv.equippedGear[1] = theItem;
                updateGearHotBar(); //update the equipped gear frame to show new gear

                //now put the replaced gear into the inventory and remove old item from inventory
                inv.removeItemFromInventory(currentHighlightItemIndex);
                inv.addItemToInventory(replacedGear.itemIndex);
            }
            if (type == ItemObjectScript.gearType.boot)
            {
                //get the gear item we are replacing
                ItemObjectScript replacedGear = inv.equippedGear[2];

                inv.equippedGear[2] = theItem;
                updateGearHotBar(); //update the equipped gear frame to show new gear

                //now put the replaced gear into the inventory and remove old item from inventory
                inv.removeItemFromInventory(currentHighlightItemIndex);
                inv.addItemToInventory(replacedGear.itemIndex);
            }

            //ALL WEAPONS -----------------------------------------------------------------------------------
            if (type == ItemObjectScript.gearType.sword)
            {
                //do we already have a gear of this type equipped?
                ItemObjectScript weapon1 = inv.equippedGear[3];
                ItemObjectScript weapon2 = inv.equippedGear[4];
                if (weapon1.itemClass == type)
                {
                    //we should replace weapon 1
                    LRUequippedweaponindex = 3;
                }
                if(weapon2.itemClass == type)
                {
                    LRUequippedweaponindex = 4;
                }



                //get the gear item we are replacing
                ItemObjectScript replacedGear = inv.equippedGear[LRUequippedweaponindex];

                inv.equippedGear[LRUequippedweaponindex] = theItem;
                updateGearHotBar(); //update the equipped gear frame to show new gear

                //now put the replaced gear into the inventory and remove old item from inventory
                inv.removeItemFromInventory(currentHighlightItemIndex);
                inv.addItemToInventory(replacedGear.itemIndex);

                LRUequippedweaponindex = (LRUequippedweaponindex % 2) + 3; //if the current value is 4 then it becomes 3 if current is 3 then it becomes 4
            }
            if (type == ItemObjectScript.gearType.bow)
            {
                //do we already have a gear of this type equipped?
                ItemObjectScript weapon1 = inv.equippedGear[3];
                ItemObjectScript weapon2 = inv.equippedGear[4];
                if (weapon1.itemClass == type)
                {
                    //we should replace weapon 1
                    LRUequippedweaponindex = 3;
                }
                if (weapon2.itemClass == type)
                {
                    LRUequippedweaponindex = 4;
                }



                //get the gear item we are replacing
                ItemObjectScript replacedGear = inv.equippedGear[LRUequippedweaponindex];

                inv.equippedGear[LRUequippedweaponindex] = theItem;
                updateGearHotBar(); //update the equipped gear frame to show new gear

                //now put the replaced gear into the inventory and remove old item from inventory
                inv.removeItemFromInventory(currentHighlightItemIndex);
                inv.addItemToInventory(replacedGear.itemIndex);

                LRUequippedweaponindex = (LRUequippedweaponindex % 2) + 3;
            }
            if (type == ItemObjectScript.gearType.shield)
            {
                //do we already have a gear of this type equipped?
                ItemObjectScript weapon1 = inv.equippedGear[3];
                ItemObjectScript weapon2 = inv.equippedGear[4];
                if (weapon1.itemClass == type)
                {
                    //we should replace weapon 1
                    LRUequippedweaponindex = 3;
                }
                if (weapon2.itemClass == type)
                {
                    LRUequippedweaponindex = 4;
                }



                //get the gear item we are replacing
                ItemObjectScript replacedGear = inv.equippedGear[LRUequippedweaponindex];

                inv.equippedGear[LRUequippedweaponindex] = theItem;
                updateGearHotBar(); //update the equipped gear frame to show new gear

                //now put the replaced gear into the inventory and remove old item from inventory
                inv.removeItemFromInventory(currentHighlightItemIndex);
                inv.addItemToInventory(replacedGear.itemIndex);

                LRUequippedweaponindex = (LRUequippedweaponindex % 2) + 3;
            }
            if (type == ItemObjectScript.gearType.magicstaff)
            {
                //do we already have a gear of this type equipped?
                ItemObjectScript weapon1 = inv.equippedGear[3];
                ItemObjectScript weapon2 = inv.equippedGear[4];
                if (weapon1.itemClass == type)
                {
                    //we should replace weapon 1
                    LRUequippedweaponindex = 3;
                }
                if (weapon2.itemClass == type)
                {
                    LRUequippedweaponindex = 4;
                }



                //get the gear item we are replacing
                ItemObjectScript replacedGear = inv.equippedGear[LRUequippedweaponindex];

                inv.equippedGear[LRUequippedweaponindex] = theItem;
                updateGearHotBar(); //update the equipped gear frame to show new gear

                //now put the replaced gear into the inventory and remove old item from inventory
                inv.removeItemFromInventory(currentHighlightItemIndex);
                inv.addItemToInventory(replacedGear.itemIndex);

                LRUequippedweaponindex = (LRUequippedweaponindex % 2) + 3;
            }

            updateItemsFrames();
            updateCharacterStats();
            //update animattions
            
        }
    }

    //invetory slots
    public Image[] items;
    public Image[] itemRars;
    
    private void updateItemsFrames()
    {
        for (int i = 0; i < 63; i++)
        {
            if (inv.scriptableInventory[i] is ItemObjectScript)
            {
                items[i].sprite = inv.scriptableInventory[i].itemPicture;
                itemSlots[i] = inv.scriptableInventory[i].itemIndex;
             
                //update rarity color
                if (inv.scriptableInventory[i].itemRarity == ItemObjectScript.gearRarity.common)
                {
                    itemRars[i].color = common;
                }
                if (inv.scriptableInventory[i].itemRarity == ItemObjectScript.gearRarity.uncommon)
                {
                    itemRars[i].color = uncommon;
                }
                if (inv.scriptableInventory[i].itemRarity == ItemObjectScript.gearRarity.rare)
                {
                    itemRars[i].color = rare;
                }
                if (inv.scriptableInventory[i].itemRarity == ItemObjectScript.gearRarity.legendary)
                {
                    itemRars[i].color = legendary;
                }
                if (inv.scriptableInventory[i].itemRarity == ItemObjectScript.gearRarity.exotic)
                {
                    itemRars[i].color = exotic;
                }
                if (inv.scriptableInventory[i].itemRarity == ItemObjectScript.gearRarity.artifact)
                {
                    itemRars[i].color = artifact;
                }
                if (inv.scriptableInventory[i].itemRarity == ItemObjectScript.gearRarity.energized)
                {
                    itemRars[i].color = energized;
                }
                
                
            }
            else
            {
                itemRars[i].color = empty;
            }
        }

    }

    //-----------------------------------------------------------------------------------------------------------------------------------------

    [Header("Gear hotbar")]
    //Gear hotbar Logic
    private ItemObjectScript helmet;
    private ItemObjectScript chest;
    private ItemObjectScript boot;
    private ItemObjectScript sword;
    private ItemObjectScript bow;
    private void getEquippedGear()
    {
        helmet = inv.equippedGear[0];
        chest = inv.equippedGear[1];
        boot = inv.equippedGear[2];
        sword = inv.equippedGear[3];
        bow = inv.equippedGear[4];
    }

    public Image helmetFrame;
    public Image chestFrame;
    public Image bootFrame;
    public Image swordFrame;
    public Image bowFrame;

    public Image helmetRar;
    public Image chestRar;
    public Image bootRar;
    public Image swordRar;
    public Image bowRar;
    private void updateGearHotBar()
    {
        getEquippedGear();
        if (helmet is ItemObjectScript)
        {
            helmetFrame.sprite = helmet.itemPicture;
            //update rarity color
            if (helmet.itemRarity == ItemObjectScript.gearRarity.common)
            {
                helmetRar.color = common;
            }
            if (helmet.itemRarity == ItemObjectScript.gearRarity.uncommon)
            {
                helmetRar.color = uncommon;
            }
            if (helmet.itemRarity == ItemObjectScript.gearRarity.rare)
            {
                helmetRar.color = rare;
            }
            if (helmet.itemRarity == ItemObjectScript.gearRarity.legendary)
            {
                helmetRar.color = legendary;
            }
            if (helmet.itemRarity == ItemObjectScript.gearRarity.exotic)
            {
                helmetRar.color = exotic;
            }
            if (helmet.itemRarity == ItemObjectScript.gearRarity.artifact)
            {
                helmetRar.color = artifact;
            }
            if (helmet.itemRarity == ItemObjectScript.gearRarity.energized)
            {
                helmetRar.color = energized;
            }
        }
        if (chest is ItemObjectScript)
        {
            chestFrame.sprite = chest.itemPicture;
            //update rarity color
            if (chest.itemRarity == ItemObjectScript.gearRarity.common)
            {
                chestRar.color = common;
            }
            if (chest.itemRarity == ItemObjectScript.gearRarity.uncommon)
            {
                chestRar.color = uncommon;
            }
            if (chest.itemRarity == ItemObjectScript.gearRarity.rare)
            {
                chestRar.color = rare;
            }
            if (chest.itemRarity == ItemObjectScript.gearRarity.legendary)
            {
                chestRar.color = legendary;
            }
            if (chest.itemRarity == ItemObjectScript.gearRarity.exotic)
            {
                chestRar.color = exotic;
            }
            if (chest.itemRarity == ItemObjectScript.gearRarity.artifact)
            {
                chestRar.color = artifact;
            }
            if (chest.itemRarity == ItemObjectScript.gearRarity.energized)
            {
                chestRar.color = energized;
            }

        }
        if (boot is ItemObjectScript)
        {
            bootFrame.sprite = boot.itemPicture;

            //update rarity color
            if (boot.itemRarity == ItemObjectScript.gearRarity.common)
            {
                bootRar.color = common;
            }
            if (boot.itemRarity == ItemObjectScript.gearRarity.uncommon)
            {
                bootRar.color = uncommon;
            }
            if (boot.itemRarity == ItemObjectScript.gearRarity.rare)
            {
                bootRar.color = rare;
            }
            if (boot.itemRarity == ItemObjectScript.gearRarity.legendary)
            {
                bootRar.color = legendary;
            }
            if (boot.itemRarity == ItemObjectScript.gearRarity.exotic)
            {
                bootRar.color = exotic;
            }
            if (boot.itemRarity == ItemObjectScript.gearRarity.artifact)
            {
                bootRar.color = artifact;
            }
            if (boot.itemRarity == ItemObjectScript.gearRarity.energized)
            {
                bootRar.color = energized;
            }

        }
        if (sword is ItemObjectScript)
        {
            swordFrame.sprite = sword.itemPicture;
            //update rarity color
            if (sword.itemRarity == ItemObjectScript.gearRarity.common)
            {
                swordRar.color = common;
            }
            if (sword.itemRarity == ItemObjectScript.gearRarity.uncommon)
            {
                swordRar.color = uncommon;
            }
            if (sword.itemRarity == ItemObjectScript.gearRarity.rare)
            {
                swordRar.color = rare;
            }
            if (sword.itemRarity == ItemObjectScript.gearRarity.legendary)
            {
                swordRar.color = legendary;
            }
            if (sword.itemRarity == ItemObjectScript.gearRarity.exotic)
            {
                swordRar.color = exotic;
            }
            if (sword.itemRarity == ItemObjectScript.gearRarity.artifact)
            {
                swordRar.color = artifact;
            }
            if (sword.itemRarity == ItemObjectScript.gearRarity.energized)
            {
                swordRar.color = energized;
            }

        }
        if (bow is ItemObjectScript)
        {
            bowFrame.sprite = bow.itemPicture;
            //update rarity color
            if (bow.itemRarity == ItemObjectScript.gearRarity.common)
            {
                bowRar.color = common;
            }
            if (bow.itemRarity == ItemObjectScript.gearRarity.uncommon)
            {
                bowRar.color = uncommon;
            }
            if (bow.itemRarity == ItemObjectScript.gearRarity.rare)
            {
                bowRar.color = rare;
            }
            if (bow.itemRarity == ItemObjectScript.gearRarity.legendary)
            {
                bowRar.color = legendary;
            }
            if (bow.itemRarity == ItemObjectScript.gearRarity.exotic)
            {
                bowRar.color = exotic;
            }
            if (bow.itemRarity == ItemObjectScript.gearRarity.artifact)
            {
                bowRar.color = artifact;
            }
            if (bow.itemRarity == ItemObjectScript.gearRarity.energized)
            {
                bowRar.color = energized;
            }

        }
    }

    //------------------------------------------------------------------------------------------------------------

    [Header("Character Stats")]
    //Update character stats
    public TMPro.TextMeshProUGUI meleeDamageT;
    public TMPro.TextMeshProUGUI rangedDamageT;
    public TMPro.TextMeshProUGUI critChanceT;
    public TMPro.TextMeshProUGUI critDamageT;
    public TMPro.TextMeshProUGUI skillDamageT;
    public TMPro.TextMeshProUGUI skillHasteT;
    public TMPro.TextMeshProUGUI armourT;


    private void updateCharacterStats()
    {
        getEquippedGear();
        float meleeDamage=0;
        float armour=0;
        float rangedDamage=0;
        double critChance=0, critDamage=0;
        double skillDamage=0, skillHaste=0;
        getEquippedGear();
        if (helmet is ItemObjectScript)
        {
            armour += helmet.armour;
            critChance += helmet.critChance;
            critDamage += helmet.critDamage;
            skillDamage += helmet.skillDamage;
            skillHaste += helmet.skillHaste;
        }
        if (chest is ItemObjectScript)
        {
            armour += chest.armour;
            critChance += chest.critChance;
            critDamage += chest.critDamage;
            skillDamage += chest.skillDamage;
            skillHaste += chest.skillHaste;
        }
        if (boot is ItemObjectScript)
        {
            armour += boot.armour;
            critChance += boot.critChance;
            critDamage += boot.critDamage;
            skillDamage += boot.skillDamage;
            skillHaste += boot.skillHaste;
        }
        if (sword is ItemObjectScript)
        {
            meleeDamage += sword.weaponDamage;
            critChance += sword.critChance;
            critDamage += sword.critDamage;
            skillDamage += sword.skillDamage;
            skillHaste += sword.skillHaste;
        }
        if (bow is ItemObjectScript)
        {
            rangedDamage += bow.weaponDamage;
            critChance += bow.critChance;
            critDamage += bow.critDamage;
            skillDamage += bow.skillDamage;
            skillHaste += bow.skillHaste;
        }

        meleeDamageT.text = "Damage: " + meleeDamage;
        rangedDamageT.text = "Ranged Damage: " + rangedDamage;
        critChanceT.text = "Critical Chance: " + critChance;
        critDamageT.text = "Critical Damage: " + critDamage;
        skillDamageT.text = "Skill Damage: " + skillDamage;
        skillHasteT.text = "Skill Haste: " + skillHaste;
        armourT.text = "Armour: " + armour;
    }

    //Called every frame (fixed rate to not give worse systems the shaft)
    void FixedUpdate()
    {
        if (ui.gameObject.activeSelf)
        {
            //player.enterUI();
        }
        else
        {
           // player.exitUI();
        }
        detectCurrentPanel();
        panelSwitching(); //detects input and moves ui with smooth transition

        itemHighlight(); //detec the highlighted slot
    }  

    //initilization
    void Start()
    {
        getEquippedGear();
        updateGearHotBar();
        updateCharacterStats();
        updateItemsFrames();

    }
}
