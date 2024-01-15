using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FriendlyAi : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float shootingInterval = 2f;
    public float maxHealth = 10f;
    public float detectionRadius = 10f;
    public LayerMask enemyLayer;
    public LayerMask healthPickupLayer;
    public float safeDistance = 5f; // Safe distance to maintain from the enemy
    public float shootingRange = 10f; // Range to start shooting

    private float currentHealth;
    private float lastShotTime = 0;
    private Transform targetEnemy;
    private GameObject targetHealthPickup;
    public float rotationSpeed = 10f; // Rotation speed towards the target
    public Transform Shootinglocaiton;

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
        // Calculate the direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Assuming the AI's forward direction aligns with Unity's X-axis, adjust the direction
        direction = Quaternion.Euler(0, -90, 0) * direction;

        // Calculate the angle in the X-axis to look up or down at the target
        float angleX = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;

        // Create a new Quaternion for the rotation, fixing X-axis and keeping Y and Z as they are
        Quaternion newRotation = Quaternion.Euler(angleX, transform.rotation.eulerAngles.y, 0);

        // Apply the rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
    }




    private void MoveTowardsTarget(Transform target)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > safeDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 newPosition = Vector3.MoveTowards(transform.position,
target.position, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
    }


    private void Shoot()
    {
        if (Time.time > lastShotTime + shootingInterval)
        {
            lastShotTime = Time.time;

            // Instantiate the bullet
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, Shootinglocaiton.position, Shootinglocaiton.rotation);

            // Adjust bullet's velocity
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                Vector3 shootingDirection = transform.forward;
                if (transform.localScale.x < 0)
                { // Adjust if the AI's scale is negative
                    shootingDirection = -shootingDirection;
                }
                bulletRb.velocity = shootingDirection * bulletSpeed;
            }
        }
    }



    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Destroy AI if health goes to 0
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPickup"))
        {
            currentHealth = Mathf.Min(currentHealth + maxHealth);
            Destroy(other.gameObject); // Assuming health pickup is to be destroyed on collection
        }
    }

    private void SeekHealthPickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, healthPickupLayer);
        float closestDistance = Mathf.Infinity;
        GameObject closestHealthPickup = null;

        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHealthPickup = hitCollider.gameObject;
            }
        }

        if (closestHealthPickup != null)
        {
            targetHealthPickup = closestHealthPickup;
            MoveTowardsTargetHealthPickup();
        }
    }

    private void MoveTowardsTargetHealthPickup()
    {
        if (targetHealthPickup != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetHealthPickup.transform.position, moveSpeed * Time.deltaTime);
            FaceTarget(targetHealthPickup.transform);
        }
    }
}