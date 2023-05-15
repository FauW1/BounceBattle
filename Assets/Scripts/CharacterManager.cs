using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public static int player1 = 0;
    public static int player2 = 0;

    [Header("Character Scriptable Objects")]
    public Character[] characters; //array

    private static GameObject instance;

    void Start()
    {
        //don't destroy or duplicate code
        if(instance == null)
        {
            DontDestroyOnLoad(this);
            instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayer1(int num, Text p1statstxt, Animator p1animator)
    {
        player1 = num;
        p1animator.SetFloat("CharNum", num/3f); //set animator to show correct demo
        p1statstxt.text = characters[num].StatsText(); //display stats
        Debug.Log($"player 1 set to {num}");
    }
    public void SetPlayer2(int num, Text p2statstxt, Animator p2animator)
    {
        player2 = num; //setting the static variable
        p2animator.SetFloat("CharNum", num / 3f); //set animator to show correct demo
        p2statstxt.text = characters[num].StatsText(); //display stats
        Debug.Log($"player 2 set to {num}");
    }

    public void StartSet(Text p1statstxt, Text p2statstxt, Animator p1animator, Animator p2animator)
    {
        p1statstxt.text = characters[player1].StatsText();
        p2statstxt.text = characters[player2].StatsText();
        p1animator.SetFloat("CharNum", player1 / 3f); //set animator to show correct demo
        p2animator.SetFloat("CharNum", player2 / 3f); //set animator to show correct demo
    }
}
