using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTag : MonoBehaviour
{
    private void Start()
    {
        if (transform.parent.tag == "Player 1")
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void Update()
    {
        if (transform.parent.rotation == Quaternion.Euler(0, 180, 0))
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        if (transform.parent.rotation == Quaternion.Euler(0, 0, 0))
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
