using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawners : MonoBehaviourPunCallbacks
{
    [Header("Enemy ")]
    public float StartTimeBtwSpawns;
    private float TimebtwSpawns;
    public Transform[] Spawnpoints;

    public Transform[] PowerupSpawnPoints;
    public float StartHealthBtwSpawns;
    private float HealthbtwSpawns;
    public GameObject Health;

    public GameObject Enemy;

    [Header("Player")]
    public GameObject Player;
    public float Minx, Miny, Maxx, Maxy;

    private bool gameStarted = false;

    void Start()
    {
        SpawnThePlayer();
        if (SinglePlayer.instance.Singleplayer)
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        StartGameIfReady();
    }

    private void StartGameIfReady()
    {
        if (!SinglePlayer.instance.Singleplayer && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2 && !gameStarted)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        gameStarted = true;
    }

    void Update()
    {
        if (gameStarted)
        {
            if (SinglePlayer.instance.Singleplayer)
            {
                UpdateEnemySpawn();
                UpdatePowerupSpawn();
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                UpdateEnemySpawn();
                UpdatePowerupSpawn();
            }
        }
    }

    private void UpdateEnemySpawn()
    {
        if (TimebtwSpawns <= 0)
        {
            Vector3 SpawnPosition = Spawnpoints[Random.Range(0, Spawnpoints.Length)].position;
            PhotonNetwork.Instantiate(Enemy.name, SpawnPosition, Quaternion.identity);

            if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                Vector3 SpawnPosition2 = Spawnpoints[Random.Range(0, Spawnpoints.Length)].position;
                PhotonNetwork.Instantiate(Enemy.name, SpawnPosition2, Quaternion.identity);
            }
            TimebtwSpawns = StartTimeBtwSpawns;
        }
        else
        {
            TimebtwSpawns -= Time.deltaTime;
        }
    }

    private void UpdatePowerupSpawn()
    {
        if (HealthbtwSpawns <= 0)
        {
            Vector3 SpawnPosition = PowerupSpawnPoints[Random.Range(0, PowerupSpawnPoints.Length)].position;
            Quaternion spawnRotation = Quaternion.Euler(0, -180, 0); // Set the y-rotation to -180 degrees
            PhotonNetwork.Instantiate(Health.name, SpawnPosition, spawnRotation);
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
