using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawners : MonoBehaviour
{
    [Header("Enemy ")]
    public float StartTimeBtwSpawns;
    float TimebtwSpawns;
    public Transform[] Spawnpoints;

    public Transform[] PowerupSpawnPoints;
    public float StartHealthBtwSpawns;
    float HealthbtwSpawns;
    public GameObject Health;

    public GameObject Enemy;

    [Header("Player")]
    public GameObject Player;
    public float Minx, Miny, Maxx, Maxy;
    // Start is called before the first frame update
    void Start()
    {
        SpawnThePlayer();
    }

    // Update is called once per frame
    void Update()
    {

        if (SinglePlayer.instance.Singleplayer)
        {
             SpawnTheEnemies();
             SpawnThePowerups();
            
            return; // Exit if it's single-player mode
        }

        if (PhotonNetwork.IsMasterClient && !PhotonNetwork.CurrentRoom.IsOpen)
        {
            // Check if there are exactly two players in the room
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                SpawnTheEnemies();
                SpawnThePowerups();
            }
        }

    }

    public void SpawnTheEnemies()
    {

            if (TimebtwSpawns <= 0)
            {
                Vector3 SpawnPosition = Spawnpoints[Random.Range(0, Spawnpoints.Length)].position;
                PhotonNetwork.Instantiate(Enemy.name, SpawnPosition, Quaternion.identity);
                TimebtwSpawns = StartTimeBtwSpawns;
            }
            else
            {
                TimebtwSpawns -= Time.deltaTime;
            }
    }

    public void SpawnThePowerups()
    {
        if (HealthbtwSpawns <= 0)
        {
            Vector3 SpawnPosition = PowerupSpawnPoints[Random.Range(0, PowerupSpawnPoints.Length)].position;
            PhotonNetwork.Instantiate(Health.name, SpawnPosition, Quaternion.identity);
            HealthbtwSpawns = StartHealthBtwSpawns;
        }
        else
        {
            HealthbtwSpawns -= Time.deltaTime;
        }
    }

    public void SpawnThePlayer()
    {
        Vector2 randomPosition = new Vector2(Random.Range(Minx, Maxx), Random.Range(Miny, Maxy));
        PhotonNetwork.Instantiate(Player.name, randomPosition, Quaternion.identity);
    }
}
