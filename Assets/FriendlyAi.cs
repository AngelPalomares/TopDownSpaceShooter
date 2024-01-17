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
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        // Convert to Euler angles to manipulate individual axis rotations
        Vector3 eulerRotation = lookRotation.eulerAngles;

        // Clamp the Y rotation between -90 and 90 degrees
        eulerRotation.y = ClampAngle(eulerRotation.y, -90f, 90f);

        // Ensure the Z rotation is within the 90s
        eulerRotation.z = 90f;

        // No need to clamp the X rotation as Quaternion.LookRotation already provides a 360-degree rotation capability

        // Convert back to Quaternion
        Quaternion constrainedRotation = Quaternion.Euler(eulerRotation);

        // Rotate smoothly towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, constrainedRotation, Time.deltaTime * rotationSpeed);
    }

    // Utility method to clamp angles between -180 and 180 degrees
    private float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        return Mathf.Clamp(angle, min, max);
    }

    // Normalize angles to be within -180 to 180 degrees range
    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
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