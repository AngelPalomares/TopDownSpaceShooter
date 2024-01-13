using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("Prefab")]
    public PlayerBullet m_prefab_player_bullet;

    [Header("Parameter")]
    public float m_move_speed = 1;
    public float bulletSpeed = 10f; // Bullet speed
    public float PlayerHealth = 4;
    public float MaxPlayerHealth = 10;

    public float AmounttoHealh;

    public float Minx, Miny, Maxx, Maxy;

    private float damageCooldown = 1.0f; // Cooldown time in seconds
    private float lastDamageTime = 0; // Time when last damage was taken

    public MeshRenderer playerMeshRenderer; // Reference to the player's mesh renderer
    public Color hitColor = Color.red; // Color to flash when hit
    private Color originalColor; // To store the original color

    public Canvas ui;
    public TMP_Text Name;

    public void Start()
    {

        ui.worldCamera = Camera.main;


        PlayerHealth = MaxPlayerHealth;
        if (photonView.IsMine)
        {
            Name.text = PhotonNetwork.NickName;
            UICanvas.instance.HealthSlider.fillAmount = (float)PlayerHealth / (float)MaxPlayerHealth;
            UICanvas.instance.Health.text = "Health :" + PlayerHealth.ToString();
        }
        else
        {
            Name.text = photonView.Owner.NickName;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            PlayerMovement();
            UpdateUI();
            UpdateTextRotation();
        }
    }

    private void UpdateTextRotation()
    {
        // Assuming the camera is always looking down and only rotates around the Y-axis
        Vector3 cameraRotation = Camera.main.transform.rotation.eulerAngles;
        Name.transform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
    }


    private void UpdateUI()
    {
        UICanvas.instance.HealthSlider.fillAmount = (float)PlayerHealth / (float)MaxPlayerHealth;
        UICanvas.instance.Health.text = "Health :" + PlayerHealth.ToString();
    }

    public void PlayerMovement()
    {
        if (photonView.IsMine)
        {
            // Look at cursor in 2D mode
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            mousePosition.z = transform.position.z; // Keep the z-position consistent with the player's z-position
            Vector3 lookDirection = mousePosition - transform.position;
            if (lookDirection.sqrMagnitude > 0.01f) // Check to avoid LookAt when mouse is very close to player
            {
                transform.LookAt(mousePosition);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 90); // Maintain z-rotation at 90 degrees
            }

            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            Vector3 movement = (transform.forward * verticalInput + transform.right * horizontalInput).normalized * m_move_speed * Time.deltaTime;

            Vector3 newPosition = transform.position + movement;

            newPosition.x = Mathf.Clamp(newPosition.x, Minx, Maxx);
            newPosition.y = Mathf.Clamp(newPosition.y, Miny, Maxy);

            transform.position = newPosition;

            // Shoot
            if (Input.GetMouseButtonUp(0))
            {
                // Instantiate the bullet with the player's current rotation
                PhotonNetwork.Instantiate(m_prefab_player_bullet.name, transform.position, transform.rotation);
            }
        }
    }


    [PunRPC]
    public void DestroyByPlayer()
    {
        if (Time.time - lastDamageTime > damageCooldown)
        {
            lastDamageTime = Time.time;

            if (photonView.IsMine)
            {
                PlayerHealth--;
                UpdateUI();

                if (PlayerHealth <= 0)
                {
                    photonView.RPC("DeactivatePlayer", RpcTarget.AllBuffered);
                }
            }

            photonView.RPC("FlashPlayer", RpcTarget.All);
        }
    }


    [PunRPC]
    public void FlashPlayer()
    {
        StartCoroutine(FlashOnHit());
    }

    private IEnumerator FlashOnHit()
    {
        int numFlashes = 5;
        float flashDelay = 0.1f;

        for (int i = 0; i < numFlashes; i++)
        {
            photonView.RPC("ToggleRenderer", RpcTarget.All);
            yield return new WaitForSeconds(flashDelay);
        }

        photonView.RPC("EnsureRendererEnabled", RpcTarget.All);
    }

    [PunRPC]
    void ToggleRenderer()
    {
        Debug.Log("ToggleRenderer called on: " + photonView.Owner.NickName);
        playerMeshRenderer.enabled = !playerMeshRenderer.enabled;
    }

    [PunRPC]
    void EnsureRendererEnabled()
    {
        Debug.Log("EnsureRendererEnabled called on: " + photonView.Owner.NickName);
        playerMeshRenderer.enabled = true;
    }


    [PunRPC]
    public void DeactivatePlayer()
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void HealThePlayer()
    {
        if (photonView.IsMine)
        {
            PlayerHealth = MaxPlayerHealth;
            UpdateUI();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        EnemyShipScript enemy = other.gameObject.GetComponent<EnemyShipScript>();
        HealthPickup health = other.gameObject.GetComponent<HealthPickup>();
        if (other.tag == "Enemy")
        {
            photonView.RPC("DestroyByPlayer", RpcTarget.All);
            enemy.photonView.RPC("RequestDestruction", RpcTarget.AllViaServer);
        }
        else if (other.tag == "Health" && photonView.IsMine)
        {
            photonView.RPC("HealThePlayer", RpcTarget.All);
            other.gameObject.GetComponent<HealthPickup>().photonView.RPC("DestroyHealthPickup", RpcTarget.MasterClient);
        }
    }

}
