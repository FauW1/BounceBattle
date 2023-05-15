using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class LostScript : MonoBehaviour
{
    private CinemachineTargetGroup targetbrain;
    void Awake()
    {
        targetbrain = GameObject.Find("TargetGroup1").GetComponent<CinemachineTargetGroup>();
        targetbrain.AddMember(transform, 1f, 5f); //cinemachine follow lost
    }
}
