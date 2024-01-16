using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FriendlyAi : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float shootingInterval = 2f;
    public float maxHealth = 10f;
    public float detectionRadius = 10f;
    public LayerMask enemyLayer;
    public float safeDistance = 5f; // Safe distance to maintain from the enemy
    public float shootingRange = 10f; // Range to start shooting
    public Transform Shootinglocaiton;
    public float rotationSpeed = 10f; // Rotation speed towards the target

    private float currentHealth;
    private float lastShotTime = 0;
    private Transform targetEnemy;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        FindAndTargetEnemy();

        if (targetEnemy != null)
        {
            FaceTarget(targetEnemy);

            if (Vector3.Distance(transform.position, targetEnemy.position) > safeDistance)
            {
                MoveTowardsTarget(targetEnemy);
            }

            if (Vector3.Distance(transform.position, targetEnemy.position) <= shootingRange)
            {
                Shoot();
            }
        }
    }

    private void FindAndTargetEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = hitCollider.transform;
            }
        }

        targetEnemy = closestEnemy;
    }

    private void FaceTarget(Transform target)
    {

        Vector3 targetDirection = target.position - Shootinglocaiton.position;


        float heightDifference = target.position.y - Shootinglocaiton.position.y;


        float horizontalDistance = Vector3.Distance(new Vector3(target.position.x, Shootinglocaiton.position.y, target.position.z), Shootinglocaiton.position);


        float angleX = Mathf.Atan2(heightDifference, horizontalDistance) * Mathf.Rad2Deg;


        Quaternion targetRotation = Quaternion.Euler(-angleX, 90, 90);


        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void MoveTowardsTarget(Transform target)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > safeDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
    }

    private void Shoot()
    {
        if (Time.time > lastShotTime + shootingInterval)
        {
            lastShotTime = Time.time;
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, Shootinglocaiton.position, Shootinglocaiton.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity= Shootinglocaiton.forward * bulletSpeed;
            }
        }
    }

public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }


}