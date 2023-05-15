using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyCharacterManager : MonoBehaviour
{
    public CharacterManager characterManage;

    [Header("Display")]
    public GameObject player1demo, player2demo;
    public Text player1stats, player2stats;
    private Animator p1anim, p2anim;

    //stores all values and uses the functions of Character Manager

    void Start()
    {
        characterManage = GameObject.FindGameObjectWithTag("Character Manager").GetComponent<CharacterManager>();

        p1anim = player1demo.GetComponent<Animator>();
        p2anim = player2demo.GetComponent<Animator>();

        characterManage.StartSet(player1stats, player2stats, p1anim, p2anim);
    }

    public void SetPlayer1(int num)
    {
        characterManage.SetPlayer1(num, player1stats, p1anim);
    }

    public void SetPlayer2(int num)
    {
        characterManage.SetPlayer2(num, player2stats, p2anim);
    }
}
