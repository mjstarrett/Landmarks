//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;
using Valve.VR;

public enum ControlOptions
{
    hold2point,
    press2toggle,
    alwaysOn
}

public class SteamVR_LaserPointer_mjs : MonoBehaviour
{
    public SteamVR_Behaviour_Pose pose;

    //public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.__actions_default_in_InteractUI;
    public SteamVR_Input_Sources inputSource;
    public ControlOptions controlBehavior = ControlOptions.hold2point;
    public SteamVR_Action_Boolean activatePointer = SteamVR_Input.GetBooleanAction("InteractUI");
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
    

    public bool active = true;
    public Color color;
    public float thickness = 0.002f;
    public Color clickColor = Color.green;
    public GameObject holder;
    public GameObject pointer;
    bool isActive = false;
    public bool addRigidBody = false;
    public Transform reference;
    public event PointerEventHandler PointerIn;
    public event PointerEventHandler PointerOut;
    public event PointerEventHandler PointerClick;

    Transform previousContact = null;


    private void Start()
    {

        

        if (pose == null)
            pose = this.GetComponent<SteamVR_Behaviour_Pose>();
        if (pose == null)
            Debug.LogError("No SteamVR_Behaviour_Pose component found on this object");
            
        if (interactWithUI == null)
            Debug.LogError("No ui interaction action has been set on this component.");
            
       

        holder = new GameObject();
        holder.name = "PointerHolder";
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;
        holder.layer = LayerMask.NameToLayer("HUD only");

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.name = "Pointer";
        pointer.transform.parent = holder.transform;
        pointer.layer = LayerMask.NameToLayer("HUD only");
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        pointer.transform.localRotation = Quaternion.identity;
        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if (collider)
            {
                Object.Destroy(collider);
            }
        }
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;

        //// MJS - add collider to the beam
        //MeshCollider boxCollider = pointer.AddComponent<MeshCollider>();
    }

    public virtual void OnPointerIn(PointerEventArgs e)
    {
        if (PointerIn != null)
            PointerIn(this, e);
    }

    public virtual void OnPointerClick(PointerEventArgs e)
    {
        if (PointerClick != null)
            PointerClick(this, e);
    }

    public virtual void OnPointerOut(PointerEventArgs e)
    {
        if (PointerOut != null)
            PointerOut(this, e);
    }


    private void Update()
    {

        //---------------------------------------------------------------------------------------------
        //--------------------- MJS - Handle the on/off of the beam------------------------------------
        //---------------------------------------------------------------------------------------------

        // Handle our pointer states
        {
            if (isActive)
            {
                pointer.SetActive(true);
            }
            else
            {
                pointer.SetActive(false);
            }
        }


        // Handle pointer on/off Behaviors based on selected interaction profile
        if (controlBehavior == ControlOptions.hold2point)
        {
            if (activatePointer.GetState(pose.inputSource))
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }

        }
        else if (controlBehavior == ControlOptions.press2toggle)
        {
            if (activatePointer.GetStateDown(inputSource))
            {
                isActive = !isActive;
            }
        }
        else if (controlBehavior == ControlOptions.alwaysOn)
        {
            if (!isActive)
            {
                isActive = true;
            }
        }
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------



        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        float dist;

        // set the distance depending on what whe're hitting
        if (Physics.Raycast(raycast, out hit))
        { 

            dist = hit.distance;
        }
        else
        {
            dist = 100f;
        }

        // draw the pointer
        if (interactWithUI != null && interactWithUI.GetState(pose.inputSource))
        {
            pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
            pointer.GetComponent<MeshRenderer>().material.color = clickColor;
        }
        else
        {
            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
            pointer.GetComponent<MeshRenderer>().material.color = color;
        }
        pointer.transform.localPosition = new Vector3(0.0f, 0.0f, dist / 2);

        // adjust for avatar scaling
        Vector3 playerScale = GameObject.FindWithTag("Player").transform.localScale;
        Vector3 temp = holder.transform.localScale;
        temp.z = 1 / playerScale.z;
        holder.transform.localScale = temp;
    }
}

public struct PointerEventArgs
{
    public SteamVR_Input_Sources fromInputSource;
    public uint flags;
    public float distance;
    public Transform target;
}

public delegate void PointerEventHandler(object sender, PointerEventArgs e);
