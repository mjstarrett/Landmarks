/*
    LM_ThirdPersonUserControl

    Modified from the ThirdPersonUserControl.cs script in standard assets.
    This script assumes special mappings for a separate player controller,
    thus allowing a large-scale player to control a remote mini avatar.

    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/

using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Valve.VR;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class LM_ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private float h;                          // Horizontal axis value from our controller to be used for movement - MJS 2019
        private float v;                          // Vertical axis value from our controller to be used for movement MJS 2019

        public SteamVR_Action_Vector2 vrTouchpad;
        public SteamVR_Action_Single squeezeTrigger;
        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            //------------------------------------
            // Standard Input
            //------------------------------------

            // read inputs -- MJS edited with ScaledNavigation button prefix to distinguish controls

            float squeezeValue = squeezeTrigger.GetAxis(SteamVR_Input_Sources.Any);
            if (squeezeValue > 0.0f)
            {
                h = vrTouchpad.GetAxis(SteamVR_Input_Sources.Any).x;
                v = vrTouchpad.GetAxis(SteamVR_Input_Sources.Any).y;
            }
            else
            {
                h = CrossPlatformInputManager.GetAxis("ScaledNavigation_Horizontal");
                v = CrossPlatformInputManager.GetAxis("ScaledNavigation_Vertical");
            }

            Debug.Log("horizontal: " + h + "\tVertical: " + v);

            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = 0.5f * (v*m_CamForward + h*m_Cam.right); // edited to make walking 
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = 0.5f * (v * Vector3.forward + h * Vector3.right);
            }

#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.RightShift)) m_Move *= 8.0f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
