using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using static UnityEditor.FilePathAttribute;
using System.Linq;

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
        FindAndTargetEnemy(); // Find the nearest enemy at the start
    }

    private void Update()
    {
        FindAndTargetEnemy(); // Continuously find the nearest enemy

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
        Transform closestEnemy = hitColliders

.Select(hitCollider => hitCollider.transform)
.OrderBy(t => Vector3.Distance(transform.position, t.position))
.FirstOrDefault();

            targetEnemy = closestEnemy;
    }

    private void FaceTarget(Transform target)
    {
        // Calculate the direction to the target
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Calculate the rotation needed to look at the target
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        // Since we are in a top-down view and we want to lock Y and Z rotations at 90 degrees,
        // we first get the X rotation from the look rotation.
        float xRotation = lookRotation.eulerAngles.x;

        // Adjust the xRotation if it goes beyond the range of -90 to 90 degrees.
        xRotation = (xRotation > 180) ? xRotation - 360 : xRotation;

        // Now we create our new rotation using only the X component of the look rotation.
        // We are assuming here that the 'up' direction for your sprites is along the global Z axis.
        Quaternion targetRotation = Quaternion.Euler(xRotation, 90, 90);

        // Slerp smoothly to the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
    Time.deltaTime * rotationSpeed);
    }



    private void MoveTowardsTarget(Transform target)
    {
        // Move the AI towards the target
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    private void Shoot()
    {
        // Handle the shooting mechanism
        if (Time.time > lastShotTime + shootingInterval)
        {
            lastShotTime = Time.time;
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, Shootinglocaiton.position, Shootinglocaiton.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = Shootinglocaiton.forward* bulletSpeed;
            }
        }
    }

public void TakeDamage(float amount)
    {
        // Handle taking damage
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}