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
    public bool defaultHandVis = true;
    public bool defaultControllerVis = true;
    private bool defaultsSaved;

    // Start is called before the first frame update
    void Start()
    {
        thisHand = transform.GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update()
    {
        // We only want this to run once when teh main render model was assigned on the last cycle
        if (!defaultsSaved && thisHand.mainRenderModel != null)
        {
            
            thisHand.mainRenderModel.SetHandVisibility(defaultHandVis);
            thisHand.mainRenderModel.SetControllerVisibility(defaultControllerVis);

            // Check for total success or we'll have to come back and do it again
            if (thisHand.mainRenderModel.IsHandVisibile() == defaultHandVis && thisHand.mainRenderModel.IsControllerVisibile() == defaultControllerVis)
            {
                defaultsSaved = true;
                Debug.LogWarning("Defaults saved");
            }
        }

        if (defaultsSaved && button.GetState(thisHand.handType))
        {
            if (Time.time - timerStart > waitForSecs)
            {
                Debug.LogWarning("The SteamVR Vive Controller was turned off by SteamVR_ToggleHand_mjsa.cs");

                if (defaultHandVis)
                {
                    thisHand.mainRenderModel.SetHandVisibility(!thisHand.mainRenderModel.IsHandVisibile());
                }
                if (defaultControllerVis)
                {
                    thisHand.mainRenderModel.SetControllerVisibility(!thisHand.mainRenderModel.IsControllerVisibile());
                }

                //// Old (will flip if only one selected
                //thisHand.mainRenderModel.SetHandVisibility(!thisHand.mainRenderModel.IsHandVisibile());
                //thisHand.mainRenderModel.SetControllerVisibility(!thisHand.mainRenderModel.IsControllerVisibile());


                timerStart = Time.time;
            }
        }
        else timerStart = Time.time;
    }
}