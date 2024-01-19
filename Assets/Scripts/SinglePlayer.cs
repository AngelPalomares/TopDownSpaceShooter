using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class SinglePlayer : MonoBehaviour
{
    public static SinglePlayer instance;

    public bool Singleplayer = false;
    public bool Tutorial = false;
    public bool GameRestart = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


}
