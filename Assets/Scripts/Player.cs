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
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    PlayerBullet bullet = Instantiate(m_prefab_player_bullet, transform.position + transform.forward, transform.rotation);
                    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                    bulletRb.velocity = transform.forward * bulletSpeed;
                }
            }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy player_bullet = collision.transform.GetComponent<Enemy>();
        if (player_bullet)
        {
            DestroyByPlayer(player_bullet);
        }
    }

    void DestroyByPlayer(Enemy a_player_bullet)
    {
        //add score
        PlayerHealth--;

        if(PlayerHealth <= 0)
        {
            this.gameObject.SetActive(false);
        }

        //delete enemy
        if (a_player_bullet)
        {
            a_player_bullet.DeleteObject();
        }


    }
}
