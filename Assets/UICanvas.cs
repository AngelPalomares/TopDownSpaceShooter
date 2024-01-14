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

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


    }
}
