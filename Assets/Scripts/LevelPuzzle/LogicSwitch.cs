using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicSwitch : MonoBehaviour
{
    public bool onOff;
    //on is green off is red

    public Sprite onSprite, offSprite;

    public PipeDetails source1;
    public PipeDetails source2;

    public enum logicType { inverter, and, or}
    public logicType type;

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


        if(type == logicType.inverter)
        {
            if (source1 != null)
            {

                //grab source details
                if (!source1.GetComponent<PipeDetails>().onOff)
                {
                    //invert it
                    onOff = true;
                }
                else
                {
                    onOff = false;
                }
            }
            else if (source1 != null)
            {
                //grab source details
                if (!source1.GetComponent<PipeDetails>().onOff)
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

        if (type == logicType.and)
        {
            if(source1.onOff && source2.onOff)
            {
                //its on
                onOff = true;
            }
            else
            {
                onOff = false;
            }
        }

        if (type == logicType.or)
        {
            if (source1.onOff && !source2.onOff || !source1.onOff && source2.onOff)
            {
                onOff = true;
            }
            else
            {
                onOff = false;
            }
        }


    }
}
