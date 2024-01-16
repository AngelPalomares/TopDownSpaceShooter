using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{

    public TMP_Text buttontext;

    private RoomInfo info;

    public void setbuttonDetails(RoomInfo inputInfo)
    {
        info = inputInfo; ;

        buttontext.text = info.Name;
    }

}
