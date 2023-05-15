using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;


public class UIManager : MonoBehaviour
{
    //camera anchor
    public GameObject cameraAnchor;

    //player data
    public GameObject p1, p2;
    private PlayerMovement m1, m2;
    private float h1, h2;

    //UI
    public Text txt;
    [SerializeField] private string message;
    private GameObject blackOutSquare;
    private GameObject health1, health2;
    private GameObject frame1, frame2;
    public GameObject buttons;

    private bool endScreenOn = false;
    void Start()
    {
        txt.enabled = false;
        buttons.SetActive(false);

        //get game objects and Vars
        m1 = p1.GetComponent<PlayerMovement>();
        m2 = p2.GetComponent<PlayerMovement>();

        blackOutSquare = GameObject.FindGameObjectWithTag("Fade Screen");

        health1 = GameObject.Find("Player 1 Health");
        health2 = GameObject.Find("Player 2 Health");

        frame1 = GameObject.Find("Power Frame 1");
        frame2 = GameObject.Find("Power Frame 2");
    }

    void Update()
    {
        h1 = m1.health;
        h2 = m2.health;
        if (h1 == 0 && !endScreenOn)
        {
            EndScreen("player 2 ", p1);
        }

        if (h2 == 0 && !endScreenOn)
        {
            EndScreen("player 1 ", p2);
        }
    }

    private void EndScreen(string playerString, GameObject player)
    {
        Instantiate(cameraAnchor, new Vector2(player.transform.position.x, player.transform.position.y), Quaternion.identity);
        Destroy(player);
        endScreenOn = true;
        StartCoroutine(FadeBlackOutSquare(playerString, 3));
        //FIND THE BUG! THE CAMERA FLIES OUT OF BOUNDS WHEN HIT BELOW
    }

    public IEnumerator FadeBlackOutSquare(string playerString, int fadeSpeed = 5)
    {
        yield return new WaitForSeconds(3);

        txt.text = playerString + message;
        txt.enabled = true;

        health1.SetActive(false);
        health2.SetActive(false);
        frame1.SetActive(false);
        frame2.SetActive(false);

        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        while (blackOutSquare.GetComponent<Image>().color.a < 0.8)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.GetComponent<Image>().color = objectColor;
            yield return null;
        }

        yield return new WaitForSeconds(3);

        buttons.SetActive(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Character Select");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title Screen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Destroy(p1);
        Destroy(p2);
        Time.timeScale = 1;
        SceneManager.LoadScene("Character Select");
    }
}
