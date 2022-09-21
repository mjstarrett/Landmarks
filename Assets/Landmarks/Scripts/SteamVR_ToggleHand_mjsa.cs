using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SteamVR_ToggleHand_mjsa : MonoBehaviour
{
    public float waitForSecs = 3.0f; // how long do the keystrokes need to be held?

    private Hand thisHand;
    public SteamVR_Action_Boolean button = SteamVR_Actions.landmarks.GripButton;
    private float timerStart;

    // Start is called before the first frame update
    void Start()
    {
        thisHand = transform.GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (button.GetState(thisHand.handType))
        {
            Debug.Log("holding...");
            if (Time.time - timerStart > waitForSecs)
            {
                Debug.LogWarning("The SteamVR Vive Controller was turned off by SteamVR_ToggleHand_mjsa.cs");
                thisHand.mainRenderModel.SetHandVisibility(!thisHand.mainRenderModel.IsHandVisibile());
                thisHand.mainRenderModel.SetControllerVisibility(!thisHand.mainRenderModel.IsControllerVisibile());
                timerStart = Time.time;
            }
        }
        else timerStart = Time.time;
    }
}