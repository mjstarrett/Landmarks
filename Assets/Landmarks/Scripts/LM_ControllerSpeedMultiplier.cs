using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_ControllerSpeedMultiplier : MonoBehaviour
{
    private CharacterController controller;
    public float speedMultiplier = 1.0f;

    void Start()
    {
        //You don't really want to use GetComponent in Update because it's slow
        //And you only need to do it once.
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 relativeForward = transform.TransformDirection(Vector3.forward);

        

    }
}