using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class EnemyShipScript : MonoBehaviourPunCallbacks
{
    public Transform[] playerTransforms;
    public float moveSpeed = 5f;
    public AudioClip DeathSound;
    public GameObject HealthPickup;

    public GameObject Shooting;

    private Transform targetPlayerTransform; 
    public float healthPickupSpawnProbability = 0.5f; 

    public bool CanShoot;

    public GameObject projectilePrefab; 
    public float shootingRate = 2f; 

    private float shootTimer;



    private void Start()
    {

        if (playerTransforms == null || playerTransforms.Length == 0)
        {
            playerTransforms = FindObjectsOfType<Player>()
                .Where(player => player.gameObject.activeSelf) 
                .Select(player => player.transform)
                .ToArray();
        }

        FindNearestPlayer();
    }

    private void Update()
    {
        if (playerTransforms != null && playerTransforms.Length > 0)
        {
            FindNearestPlayer();

            if (targetPlayerTransform != null)
            {

                MoveTowardsTargetPlayer();
            }
        }

        if (CanShoot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootingRate)
            {
                Shoot();
                shootTimer = 0f;
            }
        }
    }

    private void FindNearestPlayer()
    {
        float minDistance = float.MaxValue;
        Transform nearestPlayer = null;
        foreach (var player in playerTransforms)
        {

            if (player != null && player.gameObject.activeSelf)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPlayer = player;
                }
            }
        }
        targetPlayerTransform = nearestPlayer;
    }


    [PunRPC]
    public void PlayDeathSound()
    {
        AudioSource.PlayClipAtPoint(DeathSound, transform.position);
    }

    [PunRPC]
    public void RequestDestruction()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("PlayDeathSound", RpcTarget.All);

            if (Random.value < healthPickupSpawnProbability)
            {
                Quaternion spawnRotation = Quaternion.Euler(0, -180, 0);
                PhotonNetwork.Instantiate(HealthPickup.name, transform.position, spawnRotation);
            }

            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void MoveTowardsTargetPlayer()
    {

        if (targetPlayerTransform != null)
        {
            Vector3 directionToPlayer = (new Vector3(targetPlayerTransform.position.x, targetPlayerTransform.position.y, transform.position.z) - transform.position).normalized;
            transform.position += new Vector3(directionToPlayer.x, directionToPlayer.y, 0) * moveSpeed * Time.deltaTime;
            RotateTowardsPlayer();
        }
    }

    private void RotateTowardsPlayer()
    {

        if (targetPlayerTransform != null)
        {
            Vector3 direction = new Vector3(targetPlayerTransform.position.x, targetPlayerTransform.position.y, transform.position.z) - transform.position;
            if (direction.sqrMagnitude > 0f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = lookRotation;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 90);
            }
        }
    }

    private void Shoot()
    {
        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, transform.position, transform.rotation);
    }


}
