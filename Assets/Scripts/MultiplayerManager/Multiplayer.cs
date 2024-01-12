using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Multiplayer : MonoBehaviourPunCallbacks
{
    public TMP_InputField CreateInput, JoinINput;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatetheRoom()
    {
        RoomOptions roomoptions = new RoomOptions();
        roomoptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(CreateInput.text, roomoptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(JoinINput.text);
        
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Main Game");
    }
}
