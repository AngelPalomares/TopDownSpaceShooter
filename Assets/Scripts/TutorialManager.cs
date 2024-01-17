using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon;
using Photon.Realtime;
using Photon.Pun;

public class TutorialManager : MonoBehaviourPunCallbacks
{
    public bool[] Buttons;
    public TMP_Text Text;
    private int tutorialStep = 0;

    public GameObject Player;
    public float Minx, Miny, Maxx, Maxy;

    private Vector3 lastMousePosition;
    private float mouseMoveDistance = 0f;
    private float mouseMoveThreshold = 100f; // Adjust this threshold as needed

    // Start is called before the first frame update
    void Start()
    {
        SpawnThePlayer();
        StartCoroutine(TutorialSequence());
    }

    void Update()
    {
        switch (tutorialStep)
        {
            case 2:
                if (Input.GetKeyDown(KeyCode.W))
                    StartCoroutine(NextStepS());
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.S))
                    StartCoroutine(MoveLeft());
                break;
            case 4:
                if (Input.GetKeyDown(KeyCode.A))
                    StartCoroutine(MoveRight());
                break;
            case 5:
                if (Input.GetKeyDown(KeyCode.D))
                    StartCoroutine(ShootingStep());
                break;
            case 6:
                if (Input.GetMouseButtonDown(0)) // Left mouse button
                    StartCoroutine(AimingStep());
                break;
            case 7:
                CheckMouseMovement();
                break;
        }
    }


    private IEnumerator TutorialSequence()
    {
        Text.text = "Hello and welcome to Space Skirmish";

        yield return new WaitForSeconds(3f);

        Text.text = "Today I will be showing you how to play the game";

        yield return new WaitForSeconds(3f);

        Text.text = "To move forward, press W!";
        tutorialStep = 2;
    }

    private IEnumerator NextStepS()
    {
        Text.text = "Great! Now try moving backwards by pressing S.";

        tutorialStep = 3;
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator MoveLeft()
    {
        Text.text = "Nice! Now let's move left. Press A.";

        tutorialStep = 4;
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator MoveRight()
    {
        Text.text = "Good job! Finally, try moving right by pressing D.";

        tutorialStep = 5;
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator ShootingStep()
    {
        Text.text = "Now, let's learn to shoot! Press the left mouse button to fire.";

        tutorialStep = 6;
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator AimingStep()
    {
        Text.text = "Great! To aim, move your mouse.";

        // Reset mouse tracking values
        lastMousePosition = Input.mousePosition;
        mouseMoveDistance = 0f;

        tutorialStep = 7;
        yield return new WaitForSeconds(3f);
    }

    private void CheckMouseMovement()
    {
        Vector3 delta = Input.mousePosition - lastMousePosition;
        mouseMoveDistance += delta.magnitude;

        if (mouseMoveDistance > mouseMoveThreshold)
        {
            StartCoroutine(FinishTutorial());
        }

        lastMousePosition = Input.mousePosition;
    }


    private IEnumerator FinishTutorial()
    {
        Text.text = "Awesome! You've completed the basic movement tutorial. Enjoy the game!";
        yield return null;
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }




    public void SpawnThePlayer()
    {
        Vector2 randomPosition = new Vector2(Random.Range(Minx, Maxx), Random.Range(Miny, Maxy));
        PhotonNetwork.Instantiate(Player.name, randomPosition, Quaternion.identity);
    }
}
