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
    public float MaxPlayerHealth = 4;

    public void Start()
    {
        PlayerHealth = MaxPlayerHealth;
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

                // Calculate movement based on key inputs and ship's orientation
                float verticalInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");
                Vector3 movement = (transform.forward * verticalInput + transform.right * horizontalInput).normalized * m_move_speed * Time.deltaTime;

                // Apply movement
                transform.position += movement;

                // Shoot
                if (Input.GetMouseButtonUp(0))
                {
                    PlayerBullet bullet = Instantiate(m_prefab_player_bullet, transform.position + transform.forward, transform.rotation);
                    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                    bulletRb.velocity = transform.forward * bulletSpeed;
                }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyShipScript enemy = collision.transform.GetComponent<EnemyShipScript>();
        PlayerBullet bullet = collision.transform.GetComponent<PlayerBullet>();

        if (enemy != null)
        {
            // Destroy the enemy GameObject across all clients
            PhotonNetwork.Destroy(enemy.gameObject);

            // If this is the client that owns this player, handle health deduction
            if (photonView.IsMine)
            {
                photonView.RPC("DestroyByPlayer", RpcTarget.All, photonView.ViewID);
            }
        }
        else if (bullet != null)
        {
            // Destroy the bullet GameObject across all clients
            PhotonNetwork.Destroy(bullet.gameObject);
        }
    }

    [PunRPC]
    public void DestroyByPlayer(int viewId)
    {
        // Check if the viewId matches and if this client owns the photonView
        if (photonView.ViewID == viewId && photonView.IsMine)
        {
            PlayerHealth--;

            if (PlayerHealth <= 0)
            {
                // Destroy this gameObject across all clients
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
