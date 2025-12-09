using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Connection : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        print("ConectadoALMAster!!");
    }

    public void ButtonConnect()
    {
        RoomOptions options = new RoomOptions() { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom("room1", options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Conectado a la sala " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Hay..." + PhotonNetwork.CurrentRoom.PlayerCount + " jugadores.");
    }

    private void Update()
    {
        PhotonNetwork.LoadLevel(1);
        Destroy(this.gameObject);
    }
}
