using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLogic : MonoBehaviour
{
    public bool onOff; //true is on, false is off

    public Sprite greenLever, redLever;

    void Update()
    {
        if (onOff)
        {
            this.GetComponent<SpriteRenderer>().sprite = greenLever;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = redLever;
        }
    }
}
