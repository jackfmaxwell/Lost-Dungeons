using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeDetails : MonoBehaviour
{
    //Source can either be a switch or a pipe

    public bool onOff;
    //on is green off is red

    public Sprite onSprite, offSprite;

    public SwitchLogic source1;
    public PipeDetails source2;
    public LogicSwitch source3;

    void Update()
    {
        if (onOff)
        {
            this.GetComponent<SpriteRenderer>().sprite = onSprite;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = offSprite;
        }



        if (source1 != null)
        {
            //grab source details
            if (source1.GetComponent<SwitchLogic>().onOff)
            {
                onOff = true;
            }
            else
            {
                onOff = false;
            }
        }
        else if (source2 != null)
        {
            //grab source details
            if (source2.GetComponent<PipeDetails>().onOff)
            {
                onOff = true;
            }
            else
            {
                onOff = false;
            }
        }
        else if (source3 != null)
        {
            //grab source details
            if (source3.GetComponent<LogicSwitch>().onOff)
            {
                onOff = true;
            }
            else
            {
                onOff = false;
            }
        }
        else
        {
            //Both sources are null
        }
    }

}
