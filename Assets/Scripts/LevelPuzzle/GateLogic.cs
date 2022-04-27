using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateLogic : MonoBehaviour
{
    public Animator gateAnim;
    public bool powerActive = false;
    public bool RedGreen;//true for red, false for green

    //pipe source
    public PipeDetails source;

    public bool gateOpen=false; //true when open false when closed

    void Start()
    {
        if (RedGreen)
        {
            gateAnim.SetLayerWeight(0, 1f);
            gateAnim.SetLayerWeight(1, 0f);
        }
        if (!RedGreen)
        {
            gateAnim.SetLayerWeight(0, 0f);
            gateAnim.SetLayerWeight(1, 1f);
        }

    }

    void Update()
    {
        if (powerActive && !gateOpen)
        {
            gateOpen = true;
            this.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            gateAnim.SetTrigger("OpenGate");
        }
        else if(!powerActive && gateOpen) 
        {
            gateOpen = false;
            this.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            gateAnim.SetTrigger("CloseGate");
        }



        if (RedGreen)
        {
            if (!source.onOff)
            {
                //switch is on
                powerActive = true;
            }
            else
            {
                powerActive = false;
            }
        }
        else
        {
            if (source.onOff)
            {
                //switch is on
                powerActive = true;
            }
            else
            {
                powerActive = false;
            }
        }
       
    }
}
