using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class holds the Item Scriptable Objects in the database so they can be pulled and refrenced when needed
//some event sthat requrie this are inventory selection (highlight), inventory equipping, and loot drops (?)
public class ItemDatabase : MonoBehaviour
{
    public List<ItemObjectScript> database = new List<ItemObjectScript>();  //any size

    public List<SkillObjectScript> skilldatabase = new List<SkillObjectScript>();

    //loops through database looking for an item with matching param index
    public ItemObjectScript findItemByIndex(int index)
    {
       
        foreach (ItemObjectScript item in database)
        {
            if(item is ItemObjectScript)
            {
                if (item.itemIndex == index)
                {
                    
                    return item;
                }
            }
           
        }
        
        return null;
    }

    //loop through skill database looking for skill with matching param index
    public SkillObjectScript findSkillByIndex(int index)
    {
        foreach (SkillObjectScript skill in skilldatabase)
        {
            if (skill.skillIndex == index)
            {
                return skill;
            }
        }
        return null;
    }

    //generate a random drop
    public int getRandomDrop(ItemObjectScript.gearRarity rarity)
    {
        List<ItemObjectScript> possibleDrops = new List<ItemObjectScript>();

        foreach(ItemObjectScript item in database)
        {
            if(item.itemRarity == rarity)
            {
                //add to list of random drops
                possibleDrops.Add(item);
            }
        }

        //int i = Random.Range(0, 2);

        return 1;//possibleDrops[i].itemIndex;
        
        //return 0; 
    }



}
