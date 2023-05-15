using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    [Range(0, 3)] //this is an attribute, just like the one at the top, restricting the numbers to integers
    public int characterNumber = 0; //0 kangaroo, 1 rabbit, 2 frog, 3 puma
    private string[] animals = new string[] { "Kangaroo", "Rabbit", "Frog", "Puma" };

    public string characterName = "Kyle";

    public float speed = 200f, jumpForce = 20f, smashAcc = 700f, health = 150f, attack = 20f, specialValue = 250f, abilityDuration = 1f, coolDown = 3f;
    public string abilityname = "Sky Dash";

    public string StatsText()
    {
        return $" {characterName} \n {animals[characterNumber]} \n {health} \n {attack} \n {speed} \n {jumpForce} \n {abilityname}";
    } 
}
