using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    private const string ALIVE_PLAYERS_KEY = "AlivePlayers";

    public static GameManager Instance;
    private int totalPlayers;
    public int deadPlayers = 0;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateTotalPlayers();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdateTotalPlayers();
    }

    private void UpdateTotalPlayers()
    {
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

    }

    public void PlayerDestroyed()
    {
        photonView.RPC(nameof(PlayerHasDied), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PlayerHasDied()
    {
        deadPlayers++;
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (deadPlayers >= totalPlayers)
        {
            UICanvas.instance.GameOver();
        }
    }
}
