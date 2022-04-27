using Mirror;
using Steamworks;
using UnityEngine;


public class SteamLobby : MonoBehaviour
{
    [SerializeField] private GameObject hostButton = null;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAddressKey = "hostAddress";

    private NetworkManager netManager;
    void Start() 
    {
        netManager = GetComponent<NetworkManager>();
        if (!SteamManager.Initialized) { return; } //if steam isnt open then return

        //initialze the callbacks
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
        }
        else
        {
            Debug.LogError("STEAM: check 1 worked");
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
        }
        else
        {
            Debug.LogError("STEAM: check 2 worked");
        }
    }
    private void OnEnable()
    {
       
    }

    //UI button press to host a steam lobby (to invite friends)
    public void Host()
    {
        hostButton.SetActive(false);

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, netManager.maxConnections);
    }

    //callback when steam lobby is created
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //if the lobby creation was unsuccessful
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.SetActive(true);
            return;
        }
        
        netManager.StartHost(); //become a host in mirror

        //set the steam lobby host id as ours
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString());
    }

    //callback when game lobby join requested
    //just allow them to join
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.LogError("STEAM: Received request to join steam lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        Debug.LogError("STEAM: Entered Lobby");
        if (NetworkServer.active) { return; } //if the netserver is active then we are the host, dont need any logic to join our own lobby

        //grab the host address from the data steam is storing
        string hostAdress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);
        Debug.LogError("STEAM: host address from steam lobbby is: " + hostAdress);
        netManager.networkAddress = hostAdress;
        netManager.StartClient(); //do i need this? i added it

        hostButton.SetActive(false);
    }
}
