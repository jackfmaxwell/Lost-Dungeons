using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberCleanup : MonoBehaviour
{
    
    float destoryTimer=1f, destoryTime=0f;
    void Update()
    {
        if (destoryTime < destoryTimer)
        {
            destoryTime += 1 * Time.deltaTime;
        }
        if (destoryTime >= destoryTimer)
        {
            GameObject.Destroy(this.gameObject);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y + 2f * Time.deltaTime);
    }
}
