using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipScript : MonoBehaviour
{
    public Transform playerTransform; // Assign this in the inspector or find it dynamically
    public float moveSpeed = 5f; // How fast the enemy moves towards the player
    public int m_score = 100;

    private void Start()
    {
        // If the playerTransform is not assigned, find the player by tag or name
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player"); // Make sure your player has the "Player" tag
            if (player != null)
            {
                playerTransform = player.transform;
            }
            RotateTowardsPlayer();
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Calculate the movement vector in the X and Y direction (assuming Y is up/down in your game's coordinate system)
            Vector3 directionToPlayer = (new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z) - transform.position).normalized;

            // Move the enemy towards the player on the X and Y axes
            transform.position += new Vector3(directionToPlayer.x, directionToPlayer.y, 0) * moveSpeed * Time.deltaTime;

            // Rotate to face the player, considering only the X axis rotation
            RotateTowardsPlayer();
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z) - transform.position;

        // Ensure the direction vector is not zero before creating a rotation
        if (direction.sqrMagnitude > 0f)
        {

            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);


            transform.rotation = lookRotation;


            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 90);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerBullet player_bullet = collision.transform.GetComponent<PlayerBullet>();
        Player ThePlayer = collision.transform.GetComponent<Player>();

        if (player_bullet)
        {
            DeleteObject(player_bullet);
            //StageLoop.Instance.AddScore(m_score);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    public void DeleteObject(PlayerBullet bullet)
    {
        GameObject.Destroy(bullet);
        Destroy(gameObject);
    }
}
