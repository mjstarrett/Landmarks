//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem.Sample
{
        


    public class LM_SteamSkeletonInteractable : UIElement
    {
        public SteamVR_Input_Sources inputSource;
        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

        protected SkeletonUIOptions ui;

        protected override void Awake()
        {
            base.Awake();

            ui = this.GetComponentInParent<SkeletonUIOptions>();

            
        }

        protected override void OnButtonClick()
        {
            base.OnButtonClick();
        }
    }
}