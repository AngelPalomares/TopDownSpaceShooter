using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyShipScript : MonoBehaviour
{
    public Transform[] playerTransforms; // Assign this in the inspector with both player transforms

    public float moveSpeed = 5f;
    public int m_score = 100;

    private Transform targetPlayerTransform; // To keep track of the nearest player

    private void Start()
    {
        // Find players dynamically if not assigned
        if (playerTransforms == null || playerTransforms.Length == 0)
        {
            playerTransforms = FindObjectsOfType<Player>().Select(player => player.transform).ToArray();
        }
        // Initially set the target player
        FindNearestPlayer();
    }

    private void Update()
    {
        if (playerTransforms != null && playerTransforms.Length > 0)
        {
            FindNearestPlayer(); // Update the nearest player each frame

            if (targetPlayerTransform != null)
            {
                // Move towards the nearest player
                MoveTowardsTargetPlayer();
            }
        }
    }

    private void FindNearestPlayer()
    {
        float minDistance = float.MaxValue;
        foreach (var player in playerTransforms)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                targetPlayerTransform = player;
            }
        }
    }

    private void MoveTowardsTargetPlayer()
    {
        Vector3 directionToPlayer = (new Vector3(targetPlayerTransform.position.x, targetPlayerTransform.position.y, transform.position.z) - transform.position).normalized;
        transform.position += new Vector3(directionToPlayer.x, directionToPlayer.y, 0) * moveSpeed * Time.deltaTime;
        RotateTowardsPlayer();
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = new Vector3(targetPlayerTransform.position.x, targetPlayerTransform.position.y, transform.position.z) - transform.position;
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
