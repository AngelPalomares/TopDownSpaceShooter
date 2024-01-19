using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
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
    public float safeDistance = 5f; 
    public float shootingRange = 10f; 
    public Transform Shootinglocaiton;
    public float rotationSpeed = 10f;

    private float currentHealth;
    private float lastShotTime = 0;
    private Transform targetEnemy;


    private void Start()
    {
        currentHealth = maxHealth;
        FindAndTargetEnemy(); 
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

        Vector3 eulerRotation = lookRotation.eulerAngles;

        eulerRotation.y = ClampAngle(eulerRotation.y, -90f, 90f);

        eulerRotation.z = 90f;

        Quaternion constrainedRotation = Quaternion.Euler(eulerRotation);

        transform.rotation = Quaternion.Slerp(transform.rotation, constrainedRotation, Time.deltaTime * rotationSpeed);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        return Mathf.Clamp(angle, min, max);
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }


    private void MoveTowardsTarget(Transform target)
    {
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        transform.position = newPosition;
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
                bulletRb.velocity = Shootinglocaiton.forward* bulletSpeed;
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