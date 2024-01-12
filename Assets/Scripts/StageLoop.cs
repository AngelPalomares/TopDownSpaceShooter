using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Stage main loop
/// </summary>
public class StageLoop : MonoBehaviourPunCallbacks
{
	#region static 
	static public StageLoop Instance { get; private set; }
	#endregion

	//
	public TitleLoop m_title_loop;

	[Header("Layout")]
	public Transform m_stage_transform;
	public Text m_stage_score_text;

	[Header("Prefab")]
	public Player m_prefab_player;
    //public EnemySpawner m_prefab_enemy_spawner;

    public float StartTimeBtwSpawns;
    float TimebtwSpawns;
    public Transform[] Spawnpoints;
	public GameObject Enemy;
	public GameObject Player;

	public float Minx, Miny, Maxx, Maxy;

    //
    int m_game_score = 0;

	//------------------------------------------------------------------------------
	
	#region loop
	public void StartStageLoop()
	{
		StartCoroutine(StageCoroutine());
	}

	/// <summary>
	/// stage loop
	/// </summary>
	private IEnumerator StageCoroutine()
	{
		Debug.Log("Start StageCoroutine");

		SetupStage();

		while (true)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				//exit stage
				CleanupStage();
				m_title_loop.StartTitleLoop();
				yield break;
			}
			yield return null;
		}
	}
	#endregion


	void SetupStage()
	{
		Instance = this;

		m_game_score = 0;
		RefreshScore();

		//create player
		{
            Vector2 randomPosition = new Vector2(Random.Range(Minx, Maxx), Random.Range(Miny, Maxy));
            PhotonNetwork.Instantiate(Player.name, randomPosition,Quaternion.identity);

		}

		//create enemy spawner
		{
			
			{
				if (m_title_loop.StarttheGame == true)
				{
					if (TimebtwSpawns <= 0)
					{
						Vector3 SpawnPosition = Spawnpoints[Random.Range(0, Spawnpoints.Length)].position;
						PhotonNetwork.Instantiate(Enemy.name, SpawnPosition, Quaternion.identity);
						//Instantiate(Enemy, SpawnPosition, Quaternion.identity);
						TimebtwSpawns = StartTimeBtwSpawns;
					}
					else
					{
						TimebtwSpawns -= Time.deltaTime;
					}
				}
                //EnemySpawner spawner = Instantiate(m_prefab_enemy_spawner, m_stage_transform);
                /*
				if (spawner)
				{
					spawner.transform.position = new Vector3(-4, 4, 0);
					spawner.StartRunning();
				}
			}
			{
				//EnemySpawner spawner = Instantiate(m_prefab_enemy_spawner, m_stage_transform);
				if (spawner)
				{
					spawner.transform.position = new Vector3(4, 4, 0);
					spawner.StartRunning();
				}
				*/
            }
		}
			
	}

    public void Update()
    {
        if (TimebtwSpawns <= 0)
        {
            Vector2 randomPosition = new Vector2(Random.Range(Minx, Maxx), Random.Range(Miny, Maxy));
            Quaternion spawnRotation = Quaternion.Euler(90, 90, 90);
            Vector3 SpawnPosition = Spawnpoints[Random.Range(0, Spawnpoints.Length)].position;
            PhotonNetwork.Instantiate(Enemy.name, SpawnPosition, Quaternion.identity);
            //Instantiate(Enemy, SpawnPosition, spawnRotation);
            TimebtwSpawns = StartTimeBtwSpawns;
        }
        else
        {
            TimebtwSpawns -= Time.deltaTime;
        }
    }

    void CleanupStage()
	{
		//delete all object in Stage
		{
			for (var n = 0; n < m_stage_transform.childCount; ++n)
			{
				Transform temp = m_stage_transform.GetChild(n);
				GameObject.Destroy(temp.gameObject);
			}
		}

		Instance = null;
	}

	//------------------------------------------------------------------------------

	public void AddScore(int a_value)
	{
		m_game_score += a_value;
		RefreshScore();
	}

	void RefreshScore()
	{
		if (m_stage_score_text)
		{
			m_stage_score_text.text = $"Score {m_game_score:00000}";
		}
	}

}
