using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Spawners : MonoBehaviour
{
    [Header("Enemy ")]
    public float StartTimeBtwSpawns;
    float TimebtwSpawns;
    public Transform[] Spawnpoints;
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
        SpawnTheEnemies();
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

    public void SpawnThePlayer()
    {
        Vector2 randomPosition = new Vector2(Random.Range(Minx, Maxx), Random.Range(Miny, Maxy));
        PhotonNetwork.Instantiate(Player.name, randomPosition, Quaternion.identity);
    }
}
