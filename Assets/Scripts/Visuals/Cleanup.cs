using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleanup : MonoBehaviour
{
    public enum type { normal, pickUpItem}
    public type entityType;

    float time=0, timer=5;

    void Start()
    {
        if(entityType == type.normal)
        {
            //do nothing
        }
        if(entityType == type.pickUpItem)
        {
            timer = 15f;
        }
    }
    void Update()
    {
        if (time < timer)
        {
            time += 1 * Time.deltaTime;
        }
        if (time >= timer)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
