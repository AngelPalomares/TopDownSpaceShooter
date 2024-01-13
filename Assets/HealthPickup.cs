using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


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

    private void OnCollisionEnter(Collision collision)
    {
        EnemyShipScript enemy = collision.transform.GetComponent<EnemyShipScript>();

        if (enemy)
        {
            photonView.RPC("DestroyHealthPickup", RpcTarget.MasterClient);
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
