using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookDetails : MonoBehaviour
{

    public string[] story; //each chunk should be about 80 characters
    public int page = 1;
    public GameObject readingPanel;
    public bool opened = false;
    

    public void openBook()
    {
        if (story.Length > 0)
        {
            readingPanel.gameObject.SetActive(true);
            readingPanel.transform.position = this.transform.position + new Vector3(0f, -1.5f, 0f);
            readingPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = story[page - 1];
            opened = true;
        }

    }

    public void nextPage()
    {
        if(page >= story.Length)
        {
            //end of book
            print("end of book");
            readingPanel.gameObject.SetActive(false);
        }
        else
        {
            page += 1;
            readingPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = story[page - 1];
        }
       
    }
}
