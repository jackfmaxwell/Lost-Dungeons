using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberPopupManager : MonoBehaviour
{
    public Canvas canvas;
    public GameObject normalHitPopup, critHitPopup, healPopup;
    public GameObject blockedPopup;

   
   
    public void spawnHitPopup(bool crit, float amount, Transform location, bool playerHit)
    {
        amount = (int)amount;
        //Need to add an random offset to the transform as well
        //.5 left and right
        //-0.2 down .5 up
        float randomXOffset = UnityEngine.Random.Range(-0.2f, 0.2f);
        float randomYOffset = UnityEngine.Random.Range(-0.2f, 0.5f);
        Vector3 offset = new Vector3(location.position.x + randomXOffset, location.position.y + randomYOffset);
        if (crit)
        {
            GameObject popup = GameObject.Instantiate(critHitPopup);
            popup.transform.SetParent(canvas.transform);
            popup.transform.position = offset;
            popup.transform.GetChild(0).gameObject.GetComponent<Text>().text = "" +amount; //first is the backdrop
            popup.transform.GetChild(1).gameObject.GetComponent<Text>().text = "" + amount; //second is the front
            if (playerHit)
            {
                popup.transform.GetChild(1).gameObject.GetComponent<Text>().color = Color.red;
            }
        }
        else
        {
            GameObject popup = GameObject.Instantiate(normalHitPopup);
            popup.transform.SetParent(canvas.transform);
            popup.transform.position = offset;
            popup.transform.GetChild(0).gameObject.GetComponent<Text>().text = "" + amount; //first is the backdrop
            popup.transform.GetChild(1).gameObject.GetComponent<Text>().text = "" + amount; //second is the front
            if (playerHit)
            {
                popup.transform.GetChild(1).gameObject.GetComponent<Text>().color = Color.red;
            }
        }

       
    }

    public void spawnBlockedPopup(Transform location)
    {
        float randomXOffset = UnityEngine.Random.Range(-0.5f, 0.5f);
        float randomYOffset = UnityEngine.Random.Range(-0.2f, 0.5f);
        Vector3 offset = new Vector3(location.position.x + randomXOffset, location.position.y + randomYOffset);

        GameObject popup = GameObject.Instantiate(blockedPopup);
        popup.transform.SetParent(canvas.transform);
        popup.transform.position = offset;
    }

    public void spawnHealPopup(float amt, Transform location)
    {
        amt = (int)amt;
        float randomXOffset = UnityEngine.Random.Range(-0.5f, 0.5f);
        float randomYOffset = UnityEngine.Random.Range(-0.2f, 0.5f);
        Vector3 offset = new Vector3(location.position.x + randomXOffset, location.position.y + randomYOffset);

        GameObject popup = GameObject.Instantiate(healPopup);
        popup.transform.GetChild(0).gameObject.GetComponent<Text>().text = "" + amt; //first is the backdrop
        popup.transform.GetChild(1).gameObject.GetComponent<Text>().text = "" + amt; //second is the front

        popup.transform.SetParent(canvas.transform);
        popup.transform.position = offset;
    }
}
