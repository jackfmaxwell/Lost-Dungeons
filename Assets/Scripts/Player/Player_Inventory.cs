
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


//This class holds the player info, inventory, equipped items, level, equipped skills
public class Player_Inventory : MonoBehaviour
{

    [Header("Player Data")]
    [SerializeField]
    public ItemObjectScript[] scriptableInventory = new ItemObjectScript[63]; //INVENTORY
    [SerializeField]
    public int[] inventoryIndexs = new int[63];

    [SerializeField]
    public ItemObjectScript[] equippedGear = new ItemObjectScript[5]; //EQUIPPED GEAR (Helmet, chestplate, boot, sword, bow, shield, staff)
    [SerializeField]
    public int[] equippedGearIndexs = new int[5];
    [SerializeField]
    public int[] equippedWeaponIndecies = new int[2];

    private float maxhealth = 100;
    //getters
    public float getMaxHP()
    {
        return maxhealth;
    }


    //Level System
    public int level = 1;
    public float currentXpAmt = 0, currentXpCap = 100; //need to save current and cap

    //dungeon vars
    public int coins;

    public bool[] keys; //6 keys, white purple red green yellow blue
    public bool bossKey;

    //skills
    public SkillObjectScript[] skills = new SkillObjectScript[2];
    private int[] skillIndexs = new int[2];
    

    [Header("Refrences")]
    //Refrences
    public ItemDatabase db; //holds all items
    public Player_UI playerGameUI;

    void Update()
    {
        //if we are in the game scenes then track xp progress
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex!=1)
        {
            
            if (playerGameUI.xpSlider.value == 1f)
            {
                levelUp(currentXpAmt - currentXpCap);
            }
           
        }

    }
    //level system
    public void addXP(float amt)
    {
        print("Adding " + amt + " xp");
        currentXpAmt += amt;
        StartCoroutine(playerGameUI.countUpXPBar(amt));

        //need to check for level up by checking xpBar value (to time level up properly)

    }

    public void levelUp(float extraXP)
    {
        //need a level up effect
        //left over xp?
        currentXpAmt = 0f;
        playerGameUI.xpSlider.value = 0f;
        if (extraXP > 0f)
        {
            addXP(extraXP);
        }

        level += 1;
        currentXpCap = level * 100f;

    }



    //Returns character stats, grabs the details from each armour piece and weapons
    public float[] getCharacterStats()
    {
        float[] charStats = new float[9];
        float armour = 0;
        float swordDamage = 0;
        float bowDamage = 0;
        float magicStaffDamage = 0;
        float shieldArmour = 0;
        double critChance = 0, critDamage = 0;
        double skillDamage = 0, skillHaste = 0;
        

        ItemObjectScript helmet = equippedGear[0];
        ItemObjectScript chest = equippedGear[1];
        ItemObjectScript boot = equippedGear[2];
        ItemObjectScript weapon1 = equippedGear[3];
        ItemObjectScript weapon2 = equippedGear[4];
        

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
        
        if (weapon1 is ItemObjectScript)
        {
            if (weapon1.itemClass == ItemObjectScript.gearType.sword)
            {
                swordDamage += weapon1.weaponDamage;
            }
            if (weapon1.itemClass == ItemObjectScript.gearType.bow)
            {
                bowDamage += weapon1.weaponDamage;
            }
            if (weapon1.itemClass == ItemObjectScript.gearType.shield)
            {
                shieldArmour += weapon1.armour;
            }
            if (weapon1.itemClass == ItemObjectScript.gearType.magicstaff)
            {
                magicStaffDamage += weapon1.weaponDamage;
            }

            critChance += weapon1.critChance;
            critDamage += weapon1.critDamage;
            skillDamage += weapon1.skillDamage;
            skillHaste += weapon1.skillHaste;
        }
        if (weapon2 is ItemObjectScript)
        {
            if (weapon2.itemClass == ItemObjectScript.gearType.sword)
            {
                swordDamage += weapon2.weaponDamage;
            }
            if (weapon2.itemClass == ItemObjectScript.gearType.bow)
            {
                bowDamage += weapon2.weaponDamage;
            }
            if (weapon2.itemClass == ItemObjectScript.gearType.shield)
            {
                shieldArmour += weapon2.armour;
            }
            if (weapon2.itemClass == ItemObjectScript.gearType.magicstaff)
            {
                magicStaffDamage += weapon2.weaponDamage;
            }
            critChance += weapon2.critChance;
            critDamage += weapon2.critDamage;
            skillDamage += weapon2.skillDamage;
            skillHaste += weapon2.skillHaste;
        }
        
        charStats[0] = armour;
        charStats[1] = swordDamage;
        charStats[2] = bowDamage;
        charStats[3] = magicStaffDamage;
        charStats[4] = (float)critChance;
        charStats[5] = (float)critDamage;
        charStats[6] = (float)skillDamage;
        charStats[7] = (float)skillHaste;
        charStats[8] = shieldArmour;

        return charStats;
    }

    public float getArmour()
    {
        float[] stats = getCharacterStats();
        return stats[0];
    }

    public float getShieldArmour()
    {
        float[] stats = getCharacterStats();
        return stats[8];
    }

    //Save / Load --------------------------------------
    public int[] getInventoryIndexs()
    {
        int[] inventoryIndexs = new int[63];
        for (int i = 0; i < 63; i++)
        {
            if (scriptableInventory[i] is ItemObjectScript)
            {
                inventoryIndexs[i] = scriptableInventory[i].itemIndex;
            }
        }

        return inventoryIndexs;
    }
    public void setInventoryIndexs(int[] inv)
    {
        for (int i = 0; i < 63; i++)
        {
            inventoryIndexs[i] = inv[i];
        }
        
    }
    public void populateScriptableInventory()
    {
        for (int i = 0; i < 63; i++)
        {
            scriptableInventory[i] = db.findItemByIndex(inventoryIndexs[i]);
        }
    }


    public int[] getEquippedGearIndexs()
    {
        int[] equippedGearIndexs = new int[5];
        for (int i = 0; i < 5; i++)
        {
            if (equippedGear[i] is ItemObjectScript)
            {
                equippedGearIndexs[i] = equippedGear[i].itemIndex;
            }
        }

        return equippedGearIndexs;
    }
    //This function changing the equippedGear size :( Fixed?
    public void setEquippedGearIndexs(int[] eg)
    {
        for (int i = 0; i < 5; i++)
        {
            equippedGearIndexs[i] = eg[i];
        }
        
    }
    public void populateScriptableGear()
    {
        for (int i = 0; i < 5; i++)
        {

            if (db.findItemByIndex(equippedGearIndexs[i]) is ItemObjectScript)
            {
                equippedGear[i] = db.findItemByIndex(equippedGearIndexs[i]);
            }

        }
    }

    public int[] getSkillIndexs()
    {
        int[] skillIndexs = new int[2];
        for (int i = 0; i < 2; i++)
        {
            if (skills[i] is SkillObjectScript)
            {
                skillIndexs[i] = skills[i].skillIndex;
            }
        }

        return skillIndexs;
    }
    public void setSkillIndexs(int[] si)
    {
        skillIndexs = si;
    }
    public void populateScriptableSkills()
    {
        for (int i = 0; i < 2; i++)
        {
            skills[i] = db.findSkillByIndex(skillIndexs[i]);
        }
    }

    //-------------------------------------------------


    //This method adds an item to the inventory (both the scriptable list and the index array)
    //Finds first empty position in the array/scriptable inv and adds the item to it
    //find the scriptable object to add using the databases
    public void addItemToInventory(int index)
    {
        int firstEmptyIndex = System.Array.IndexOf(inventoryIndexs, 0);
        if (firstEmptyIndex == -1)
        {
            //The iventory is full
            print("Inventory is full! Cannot add item!");
        }
        else
        {
            inventoryIndexs[firstEmptyIndex] = index;

            //scriptable inventory update
            int firstEmptyScriptable = System.Array.IndexOf(scriptableInventory, null);
            scriptableInventory[firstEmptyScriptable] = db.findItemByIndex(index);
            print("Added item");
        }


    }

    //This method removes an item from the inventory (both scriptable and index)
    public void removeItemFromInventory(int index)
    {
        print("Looking for index " + index);
        for (int i = 0; i < 63; i++)
        {
            if (inventoryIndexs[i] == index)
            {
                inventoryIndexs[i] = 0;
                //scriptable inventory update
                for (int k = 0; k < 63; k++)
                {
                    if (scriptableInventory[k] is ItemObjectScript)
                    {
                        if (scriptableInventory[k].itemIndex == index)
                        {
                            //remove it
                            print("Found item to remove");
                            scriptableInventory[k] = null;
                            return;
                        }
                    }

                }

            }
        }




    }


    //This method is needed for initialize because I want to set items in the inventory from the editor (in full game that would not be needed)
    void Start()
    {
        db = GameObject.FindGameObjectWithTag("Manager").GetComponent<ItemDatabase>();


        //Takes scriptable inventory (set in editor) and translates to inventoryIndex []
        for (int i = 0; i < 63; i++)
        {
            if (scriptableInventory[i] is ItemObjectScript)
            {
                int firstEmptyIndex = System.Array.IndexOf(inventoryIndexs, 0);
                if (firstEmptyIndex == -1)
                {
                    //There is no empty index!, so just dont do anything
                    //Happening because load game is working?
                }
                else
                {
                    inventoryIndexs[firstEmptyIndex] = scriptableInventory[i].itemIndex;
                }


            }
        }

        //Takes the equippedGear scriptable list and translates to item index array
        for (int i = 0; i < 5; i++)
        {
            if (equippedGear[i] is ItemObjectScript)
            {
                int firstEmptyIndex = System.Array.IndexOf(equippedGearIndexs, 0);
                if (firstEmptyIndex == -1)
                {
                    //There is no empty index!, so just dont do anything
                    //Happening because load game is working?
                }
                else
                {
                    equippedGearIndexs[firstEmptyIndex] = equippedGear[i].itemIndex;
                }
            }
        }


    }
}


