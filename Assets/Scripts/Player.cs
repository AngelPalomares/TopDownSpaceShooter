using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("Prefab")]
    public PlayerBullet m_prefab_player_bullet;

    [Header("Parameter")]
    public float m_move_speed = 1;
    public float bulletSpeed = 10f; // Bullet speed
    public float PlayerHealth = 4;
    public float MaxPlayerHealth = 10;

    public float AmounttoHealh;

    public float Minx, Miny, Maxx, Maxy;

    public void Start()
    {
        PlayerHealth = MaxPlayerHealth;
        UICanvas.instance.Health.text = "Health :"+ PlayerHealth.ToString();
    }

    private void Update()
    {
        PlayerMovement();
    }


    public void PlayerMovement()
    {
        if (photonView.IsMine)
        {
                // Look at cursor in 2D mode
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
                mousePosition.z = transform.position.z; // Keep the z-position consistent with the player's z-position
                Vector3 lookDirection = mousePosition - transform.position;
                if (lookDirection.sqrMagnitude > 0.01f) // Check to avoid LookAt when mouse is very close to player
                {
                    transform.LookAt(mousePosition);
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 90); // Maintain z-rotation at 90 degrees
                }

   
                float verticalInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");
                Vector3 movement = (transform.forward * verticalInput + transform.right * horizontalInput).normalized * m_move_speed * Time.deltaTime;
 
            Vector3 newPosition = transform.position + movement;

            newPosition.x = Mathf.Clamp(newPosition.x, Minx, Maxx);
            newPosition.y = Mathf.Clamp(newPosition.y, Miny, Maxy);

            transform.position = newPosition;

            // Shoot
            if (Input.GetMouseButtonUp(0))
            {
                // Instantiate the bullet with the player's current rotation
                PhotonNetwork.Instantiate(m_prefab_player_bullet.name, transform.position, transform.rotation);
            }
        }
    }

    [PunRPC]
    public void DestroyByPlayer()
    {
        // Ensure that the RPC affects only the player that owns this PhotonView
        if (photonView.IsMine)
        {
            PlayerHealth--;
            UICanvas.instance.Health.text = "Health :" + PlayerHealth.ToString();

            if (PlayerHealth <= 0)
            {
                // Instead of destroying, set the gameObject to inactive
                gameObject.SetActive(false);
            }
        }

    }

    [PunRPC]
    public void HealThePlayer()
    {
        if(photonView.IsMine)
        {
            PlayerHealth += AmounttoHealh;
            PlayerHealth = Mathf.Min(PlayerHealth, MaxPlayerHealth); // Prevent overhealing
            UICanvas.instance.Health.text = "Health :" + PlayerHealth.ToString();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        EnemyShipScript enemy = other.gameObject.GetComponent<EnemyShipScript>();
        HealthPickup health = other.gameObject.GetComponent<HealthPickup>();
        if (other.tag == "Enemy")
        {
            photonView.RPC("DestroyByPlayer", RpcTarget.All);
            enemy.photonView.RPC("RequestDestruction", RpcTarget.AllViaServer);
        }
        else if (other.tag == "Health" && photonView.IsMine)
        {
            photonView.RPC("HealThePlayer", RpcTarget.All);
            // Request the MasterClient to destroy the health pickup
            other.gameObject.GetComponent<HealthPickup>().photonView.RPC("DestroyHealthPickup", RpcTarget.MasterClient);
        }
    }

}
