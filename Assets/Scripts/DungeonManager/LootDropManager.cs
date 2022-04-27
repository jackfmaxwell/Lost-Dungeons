using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LootDropManager : MonoBehaviour
{
    public Player_Inventory inv;

    //particle systems instead
    public GameObject dropPrefab;
    [ColorUsage(true, true)]
    public Color commonC, uncommonC, rareC, legendaryC, exoticC, artifactC, energizedC;
    [ColorUsage(true, true)]
    public Color commonC2, uncommonC2, rareC2, legendaryC2, exoticC2, artifactC2, energizedC2;

    public enum dropType { chest, enemy, secretChest, boss }
    public void spawnLootDrop(Transform transform, int dungeonDifficulty, dropType drop)
    {
        /*
        float r = Random.value;
        r += (float)dungeonDifficulty / 10f; //up chances by 10% for every dungeon difficulty
        
        if (drop == dropType.chest)
        {
            r += 0.1f;
        }
        if (drop == dropType.enemy)
        {
            r -= 0.2f;
        }
        if (drop == dropType.secretChest)
        {
            r += 0.2f;
        }
        if (drop == dropType.boss)
        {
            r += 0.35f;
        }



      
        if (r > 0.4)
        {
            //common
            spawnDrop(transform, rarity.common);
        }
        if (r > 0.65)
        {
            //uncommon
            spawnDrop(transform, rarity.uncommon);
        }
        if (r > 0.75)
        {
            //rare
            spawnDrop(transform, rarity.rare);
        }
        if (r > 0.84)
        {
            //legendary
            spawnDrop(transform, rarity.legendary);
        }
        if (r > 0.96)
        {
            //exotic
            spawnDrop(transform, rarity.exotic);
        }
        if (r > 0.99)
        {
            //1%
            //ENERGIZED OR ARTIFACT

            //do a 50 50
            float fifty = Random.value;
            if (fifty > 0.5)
            {
                //energized
                spawnDrop(transform, rarity.energized);
            }
            else
            {
                //artifact
                spawnDrop(transform, rarity.artifact);
            }

           
        }

        */
        spawnDrop(transform, rarity.energized);
        //spawnDrop(transform, rarity.energized);
        //spawnDrop(transform, rarity.exotic);
    }
    public void hitDrop(GameObject g)
    {
        //inv.addItemToInventory(g.transform.parent.GetComponent<LootDropDetails>().dropIndex);
        g.transform.parent.GetComponent<LootDropDetails>().startedFade = true;
    }
 
 
    public enum rarity { common, uncommon, rare, legendary, exotic, artifact, energized }
    public rarity dropRarity;
    private void spawnDrop(Transform t, rarity r)
    {
       
        if (r == rarity.common)
        {
            GameObject d1 = GameObject.Instantiate(dropPrefab);
            print(d1.transform.gameObject.name);
            d1.transform.position = t.transform.position;
          
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_74628DF3", commonC);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Fade", 1f);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_5B7739BE", commonC2);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_TotalGlow", 1f);




            d1.transform.GetComponent<LootDropDetails>().dropIndex = inv.db.getRandomDrop(ItemObjectScript.gearRarity.common);

        }


        if (r == rarity.uncommon)
        {
            GameObject d1 = GameObject.Instantiate(dropPrefab);
            print(d1.transform.gameObject.name);
            d1.transform.position = t.transform.position;
            
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_74628DF3", uncommonC);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Fade", 1f);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_5B7739BE", uncommonC2);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_TotalGlow", 1f);




            d1.transform.GetComponent<LootDropDetails>().dropIndex = inv.db.getRandomDrop(ItemObjectScript.gearRarity.uncommon);

        }

        if (r == rarity.rare)
        {
            GameObject d1 = GameObject.Instantiate(dropPrefab);
            print(d1.transform.gameObject.name);
            d1.transform.position = t.transform.position;

            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_74628DF3", rareC);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Fade", 1f);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_5B7739BE", rareC2);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_TotalGlow", 1f);




            d1.transform.GetComponent<LootDropDetails>().dropIndex = inv.db.getRandomDrop(ItemObjectScript.gearRarity.rare);

        }

        if (r == rarity.legendary)
        {
            GameObject d1 = GameObject.Instantiate(dropPrefab);
            print(d1.transform.gameObject.name);
            d1.transform.position = t.transform.position;

            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_74628DF3", legendaryC);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Fade", 1f);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_5B7739BE", legendaryC2);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_TotalGlow", 1f);




            d1.transform.GetComponent<LootDropDetails>().dropIndex = inv.db.getRandomDrop(ItemObjectScript.gearRarity.legendary);

        }

        if (r == rarity.exotic)
        {
            GameObject d1 = GameObject.Instantiate(dropPrefab);
            print(d1.transform.gameObject.name);
            d1.transform.position = t.transform.position;

            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_74628DF3", exoticC);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Fade", 1f);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_5B7739BE", exoticC2);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_TotalGlow", 0.7f);




            d1.transform.GetComponent<LootDropDetails>().dropIndex = inv.db.getRandomDrop(ItemObjectScript.gearRarity.exotic);

        }



        if (r == rarity.artifact)
        {
            GameObject d1 = GameObject.Instantiate(dropPrefab);
            print(d1.transform.gameObject.name);
            d1.transform.position = t.transform.position;

            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_74628DF3", artifactC);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Fade", 1f);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_5B7739BE", artifactC2);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_TotalGlow", -0.33f);




            d1.transform.GetComponent<LootDropDetails>().dropIndex = inv.db.getRandomDrop(ItemObjectScript.gearRarity.artifact);

        }
        if (r == rarity.energized)
        {
            GameObject d1 = GameObject.Instantiate(dropPrefab);
            print(d1.transform.gameObject.name);
            d1.transform.position = t.transform.position;

            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_74628DF3", energizedC);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Fade", 1f);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("Color_5B7739BE", energizedC2);
            d1.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_TotalGlow", -0.33f);




            d1.transform.GetComponent<LootDropDetails>().dropIndex = inv.db.getRandomDrop(ItemObjectScript.gearRarity.energized);

        }
        



    }


}
