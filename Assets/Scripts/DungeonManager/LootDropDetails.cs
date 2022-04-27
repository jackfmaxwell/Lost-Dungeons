using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropDetails : MonoBehaviour
{
    public Player_Inventory inv;
    public int dropIndex=1;

    public bool startedFade=false;

    public float timer=30f;
    //30 second life then despawn and add to inv
    void Update()
    {
        if (timer > 0f)
        {
            timer -= 1f * Time.deltaTime;
        }
        else
        {
            //delete

            inv.addItemToInventory(dropIndex);
            GameObject.Destroy(this.transform.gameObject);

        }
        if (this.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.GetFloat("_Fade") == 0f)
        {
            //remove it
            GameObject.Destroy(this.transform.gameObject);
        }
        if (startedFade)
        {
            
            this.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Fade", Mathf.Lerp(this.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.GetFloat("_Fade"), 0f, 1.5f*Time.deltaTime));
           
        }
    }
}
