using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Player Bullet
/// </summary>
public class PlayerBullet : MonoBehaviourPunCallbacks
{
	[Header("Parameter")]
	public float m_move_speed = 5f;
	public float m_life_time = 4f;

	//
	void Update()
	{
        // Move the bullet forward in the direction it's facing
        transform.position += transform.forward * m_move_speed * Time.deltaTime;

        // Existing lifetime logic
        m_life_time -= Time.deltaTime;
        if (m_life_time <= 0)
        {
            DeleteObject();
        }
    }

    public void DeleteObject()
    {
        // Ensure that the bullet is destroyed network-wide
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyShipScript enemy = other.gameObject.GetComponent<EnemyShipScript>();
            if (enemy != null && photonView.IsMine)
            {
                // Request destruction of the enemy ship via an RPC
                enemy.photonView.RPC("RequestDestruction", RpcTarget.AllViaServer);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
