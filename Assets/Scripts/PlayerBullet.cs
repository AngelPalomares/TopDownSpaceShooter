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

                enemy.photonView.RPC("RequestDestruction", RpcTarget.AllViaServer);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
