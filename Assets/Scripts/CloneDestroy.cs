using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = .6f;

    void Start()
    {
        Destroy(gameObject, lifeTime); //removes game object in some seconds
    }
}
