
using UnityEngine;

//This method handles the enemy view box, it relays the info to the enemy class
public class EnemyViewManager : MonoBehaviour
{ 
    //Set to the super class implementing Enemy_Generic, had to set protection level so it wasnt private by default
    public Enemy_Generic owner;

    //Trigger check for targets and allies
    void OnTriggerExit2D(Collider2D collision)
    {

        //remove from target list
        if (collision.gameObject.tag == "Player")
        {
            owner.lostSightOfTarget(collision.gameObject.transform);
        }
        if (collision.gameObject.tag == "Enemy" && collision.gameObject != this.gameObject)
        {
            owner.lostSightOfAlly(collision.gameObject.transform);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        //add to target list
        if (collision.gameObject.tag == "Player")
        {
            owner.spottedTarget(collision.gameObject.transform);
        }
        if (collision.gameObject.tag == "Enemy" && collision.gameObject != this.gameObject)
        {
            owner.spottedAlly(collision.gameObject.transform);
        }
    }
    
}
