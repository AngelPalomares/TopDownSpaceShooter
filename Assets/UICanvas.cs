using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UICanvas : MonoBehaviourPunCallbacks
{
    public static UICanvas instance;
    public Text HighScore;
    public Text Health;

    public TMP_Text Timer;

    public Image HealthSlider;

    public float High;

    public float Timertocount;

    public bool TimeisOver;

    public AudioClip MenuSound;

    public GameObject GameOverPanel;
    public TMP_Text GameOverText;

    public GameObject Photon;
    public GameObject Singleplayergameobject;
    SinglePlayer[] singlePlayers;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        singlePlayers = GameObject.FindObjectsOfType<SinglePlayer>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void ReturnToMenu()
    {
        AudioSource.PlayClipAtPoint(MenuSound, transform.position);
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }
    public void QuitGame()
    {
        AudioSource.PlayClipAtPoint(MenuSound, transform.position);
        Application.Quit();
    }

    public void RestartTheGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    

}
