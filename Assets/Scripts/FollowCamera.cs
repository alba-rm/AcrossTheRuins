using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform followPlayer;
    private Vector3 positionCamera = new Vector3(0,2.8f,-8);
 
    void Update()
    {
       transform.position = followPlayer.transform.position + positionCamera;
    }
}
