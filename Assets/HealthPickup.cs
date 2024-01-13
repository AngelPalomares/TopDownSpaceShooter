using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Pun;

public class HealthPickup : MonoBehaviourPunCallbacks
{
    public float Amounttoheal = 2f;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.transform.GetComponent<Player>();

        if(other.tag == "Player")
        {
            player.HealThePlayer();
        }
    }

    [PunRPC]
    public void DestroyHealthPickup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

}
