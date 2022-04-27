using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private GameObject lobbyUI = null;
   
    
    
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button readyUpButton = null;

    [Scene] [SerializeField] private string mainMenuScene = null;

    
    public string DisplayName = "Loading...";
    
    private void Awake()
    {
        DisplayName = "test name";
        if (!SteamManager.Initialized) { return; } //if steam isnt open then return
        DisplayName = SteamFriends.GetPersonaName();

    }
    
    private void FixedUpdate()
    {
        if (isLocalPlayer && SceneManager.GetActiveScene().path==mainMenuScene)
        {
            updateUI();
            lobbyUI.SetActive(true);
        }
        else
        {
            lobbyUI.SetActive(false);
        }
    }
    
    private void updateUI()
    {
        //called for everyone, so everyone sees everyones additions
        NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
        if (room)
        {

            if (!NetworkManager.IsSceneActive(room.RoomScene))
                return;

            int i = 0;
            foreach(NetRoomPlayer player in room.roomSlots)
            {   
                playerNameTexts[i].text = player.DisplayName;
                if (player.readyToBegin)
                {
                    playerReadyTexts[i].text = "<color=green>Ready</color>";
                }
                else
                {
                    playerReadyTexts[i].text = "<color=red>Not Ready</color>";
                }
                i += 1;
            }
            
            
            //this is host, show them the start button, plus remove buttons
            if (((isServer && index > 0) || isServerOnly))
            {
                
                GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
            }
            
        }

        //called on local player so we only see one ready button
        if(NetworkClient.active && isLocalPlayer)
        {
            readyUpButton.gameObject.SetActive(true);
        }

    }

    public void readyButtonPress()
    {
        print("ready pressed");
      
        
        if(readyUpButton.gameObject.name == "Ready Up")
        {
            CmdChangeReadyState(true);
            readyUpButton.gameObject.name = "Cancel";
            readyUpButton.GetComponentInChildren<TMP_Text>().text = "Cancel";
            readyUpButton.GetComponent<Image>().color = Color.red;
        }
        else if (readyUpButton.gameObject.name == "Cancel")
        {
            CmdChangeReadyState(false);
            readyUpButton.gameObject.name = "Ready Up";
            readyUpButton.GetComponentInChildren<TMP_Text>().text = "Ready Up";
            readyUpButton.GetComponent<Image>().color = Color.green;
        }
        
    }
    /*
    
    [Command]
    public void CmdReadyButtonPress()
    {
        readyToBegin = !readyToBegin; //toggle
        //get the singleton and tell it a client changed their ready status
        NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
        if (room != null)
        {
            room.ReadyStatusChanged();
            room.roomSlots[this.index].readyToBegin = readyToBegin;
        }
        
    }

 */
}
