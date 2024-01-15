using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class Multiplayer : MonoBehaviourPunCallbacks
{
    public TMP_Text Loadingtext,errortext;
    public TMP_InputField CreateInput, JoinINput, NameInput;

    public GameObject SinglePlayerPanel,MultiplayerPanel, loadingPanel;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        StartCoroutine(Connecttotheserver());
    }

    public IEnumerator Connecttotheserver()
    {
        Loadingtext.text = "Connecting to online servers!";
        loadingPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        Loadingtext.text = "Succesfully Connected";
        yield return new WaitForSeconds(1);
    }

    public override void OnConnectedToMaster()
    {
        Loadingtext.text = "Succesfully Connected";
        loadingPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inputyourname()
    {
        PhotonNetwork.NickName = NameInput.text;
    }

    public void CreatetheRoom()
    {
        if (!string.IsNullOrEmpty(NameInput.text) && !string.IsNullOrEmpty(CreateInput.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(CreateInput.text, roomOptions);
        }
        else
        {
            // Optionally, display a message to the user indicating that both fields are required.
            Debug.Log("Both Name and Room Name are required to create a room.");
            StartCoroutine(Erroroncreateroom());
        }
    }

    public IEnumerator Erroroncreateroom()
    {
        errortext.text = "Both Name and Room Name are required to create a room.";
        yield return new WaitForSeconds(2f);
        errortext.text = " ";
    }

    public IEnumerator Erroronjoinroom()
    {
        errortext.text = "Both Name and Room Name are required to join a room.";
        yield return new WaitForSeconds(2f);
        errortext.text = " ";
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrEmpty(NameInput.text) && !string.IsNullOrEmpty(JoinINput.text))
        {
            PhotonNetwork.JoinRoom(JoinINput.text);
        }
        
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Main Game");
    }

    public void CreateSinglePlayer()
    {
        SinglePlayer.instance.Singleplayer = true;
        RoomOptions roomoptions = new RoomOptions();
        roomoptions.MaxPlayers = 1;
        string randomRoomName = "Room_" + Guid.NewGuid().ToString();
        PhotonNetwork.CreateRoom(randomRoomName,roomoptions);
    }

    public void OpenMultiplayer()
    {
        SinglePlayerPanel.SetActive(false);
        MultiplayerPanel.SetActive(true);
    }
}
