using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoControl : MonoBehaviour
{
    public Animator anim1, anim2;
    public Animator w, a, s, d, up, down, left, right, r, slash;

    public Text p1Text, p2Text;

    [Header("Cycle Time")]
    public float sec = 4f;

    //public static int[] cycles = { 0, 1, 2, 3 }; //cycle these states for demo; 0 = walk, 1 = jump, 2 = squish, 3 = power
    private int cycleIndex = 0;

    private void Start()
    {
        StartCoroutine("DemoCycle");
    }

    private void Update()
    {
        anim1.SetInteger("Cycles", cycleIndex);
        anim2.SetInteger("Cycles", cycleIndex);

        //change key animations and info panel name
        if(cycleIndex == 0) //move keys
        {
            KeysActive(a, d);
            KeysActive(left, right);
            KeysInactive(w, s, up, down, r, slash);
            ChangeText(p1Text, "Walk");
            ChangeText(p2Text, "Walk");

        }
        
        if (cycleIndex == 1) //move keys
        {
            KeysActive(w, up);
            KeysInactive(a, s, d, down, left, right, r, slash);
            ChangeText(p1Text, "Jump");
            ChangeText(p2Text, "Jump");
        }
        
        if (cycleIndex == 2) //move keys
        {
            KeysActive(s, down);
            KeysInactive(w,a,d,up,left,right,r,slash);
            ChangeText(p1Text, "Drop");
            ChangeText(p2Text, "Drop");
        }
        
        if (cycleIndex == 3) //move keys
        {
            KeysActive(r, slash);
            KeysInactive(w, a, s, d, up, down, left, right);
            ChangeText(p1Text, "Special");
            ChangeText(p2Text, "Special");
        }
    }

    private void KeysActive(Animator x, Animator y) 
    {
        x.SetBool("Active", true);
        y.SetBool("Active", true);
    }

    //overloads
    private void KeysInactive(Animator x, Animator y, Animator z, Animator p, Animator u, Animator i)
    {
        x.SetBool("Active", false);
        y.SetBool("Active", false);
        z.SetBool("Active", false);
        p.SetBool("Active", false);
        u.SetBool("Active", false);
        i.SetBool("Active", false);
    }

    private void KeysInactive(Animator x, Animator y, Animator z, Animator p, Animator u, Animator i, Animator f, Animator g)
    {
        x.SetBool("Active", false);
        y.SetBool("Active", false);
        z.SetBool("Active", false);
        p.SetBool("Active", false);
        u.SetBool("Active", false);
        i.SetBool("Active", false);
        f.SetBool("Active", false);
        g.SetBool("Active", false);
    }

    private void ChangeText(Text t, string s)
    {
        t.text = s;
    }

    IEnumerator DemoCycle() //add one to cycle every four seconds
    {
        while (SceneManager.GetActiveScene().name == "Character Select") {
            if (cycleIndex < 3)
            {
                cycleIndex++;
            }
            else
            {
                cycleIndex = 0;
            }
            yield return new WaitForSeconds(sec);
        }
    }

    public void Go()
    {
        SceneManager.LoadScene("Forest");
    }

    public void Back()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
