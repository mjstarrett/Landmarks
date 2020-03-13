﻿/*
    Copyright (C) 2018 Michael James Starrett

    Navigate by Starlite - Powered by Landmarks

    This is modified from InstructionsTask.cs to incorporate gui elements such as
    a subjective slider for confidence ratings   
*/   

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class Confidence : ExperimentTask {

    [Header("Task-specific Properties")]

    public TextAsset instruction;
    public TextAsset message;
    
    public ObjectList objects;
    private GameObject currentObject;
    
    public TextList texts;
    private string currentText;
        
    public bool blackout = true;
    public Color text_color = Color.white;
    public Font instructionFont;
    public int instructionSize = 12;

    public bool actionButtonOn = true;
    public string customButtonText = "";
        
    private Text gui;

    public bool restrictMovement = false; // MJS do we want to keep them still during this?

    private GameObject sliderObject;
    private LM_vrSlider vrSlider;
    private Slider slider;
    public bool randomStartValue = true;

    private float startTime;
    
    void OnDisable ()
    {
        if (gui)
            DestroyImmediate (gui.gameObject);
    }
    
    public override void startTask () {
        TASK_START();
        Debug.Log ("Starting a Confidence Rating Task");
        ResetHud();
    }    

    public override void TASK_START()
    {
        
        if (!manager) Start();
        base.startTask();

        startTime = Time.time;
        
        if (skip) {
            log.log("INFO    skip task    " + name,1 );
            return;
        }
        

        GameObject sgo = new GameObject("Instruction Display");

        GameObject avatar = manager.player.GetComponent<HUD>().Canvas as GameObject;
        Text canvas = avatar.GetComponent<Text>();
        hud.SecondsToShow = hud.InstructionDuration;

            
        sgo.AddComponent<Text>();
        sgo.hideFlags = HideFlags.HideAndDontSave;
        sgo.transform.position = new Vector3(0,0,0);
        gui = sgo.GetComponent<Text>(); 
        // DEPRICATED IN UNITY 2019 // gui.pixelOffset = new Vector2( 20, Screen.height - 20);
        gui.font = instructionFont;
        gui.fontSize = instructionSize;
        gui.material.color = text_color;
        gui.text = message.text;                   

        if (texts) currentText = texts.currentString().Trim();
        if (objects) currentObject = objects.currentObject();
        if (instruction) canvas.text = instruction.text;
        if (blackout) hud.showOnlyHUD();
        if (message) {
            string msg = message.text;
            if (currentText != null) msg = string.Format(msg, currentText);
            if (currentObject != null) msg = string.Format(msg, currentObject.name);
            hud.setMessage(msg);
        }
        hud.flashStatus("");

        if (restrictMovement)
        {
            manager.player.GetComponent<CharacterController>().enabled = false;
            manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = true;
        }

        // Change text and turn on the map action button if we're using it
        if (actionButtonOn)
        {
            // Use custom text for button (if provided)
            if (customButtonText != "") actionButton.GetComponentInChildren<Text>().text = customButtonText;
            // Otherwise, use default text attached to the button (component)
            else actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;

            // activate the button
            hud.actionButton.SetActive(true);
            hud.actionButton.GetComponent<Button>().onClick.AddListener(hud.OnActionClick);

            // make the cursor functional and visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //---------------------------
        // Confidence Slider
        //---------------------------
        sliderObject = hud.confidenceSlider.gameObject;
        sliderObject.SetActive(true);
        if (vrEnabled)
        {
            vrSlider = sliderObject.GetComponent<LM_vrSlider>();
        }
        else
        {
            slider = sliderObject.GetComponent<Slider>();
        }

        // Reset the value before the trial starts
        if (vrEnabled)
        {
            vrSlider.ResetSliderPosition(randomStartValue);
        }
        else
        {
            if (randomStartValue) slider.value = Random.Range(slider.minValue, slider.maxValue);
            else slider.value = 0;
        }

    }
    // Update is called once per frame
    public override bool updateTask () {
        
        if (skip) {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }
        if ( interval > 0 && Experiment.Now() - task_start >= interval)  {
            return true;
        }

        //------------------------------------------
        // Handle buttons to advance the task - MJS
        //------------------------------------------
        if (hud.actionButtonClicked == true)
        {
            hud.actionButtonClicked = false;
            log.log("INPUT_EVENT    clear text    1", 1);
            return true;
        }

        if (killCurrent == true)
        {
            return KillCurrent();
        }

        return false;
    }
    
    public override void endTask() {
        TASK_END();
    }
    
    public override void TASK_END() {
        base.endTask ();
        hud.setMessage ("");
        hud.SecondsToShow = hud.GeneralDuration;

        // Save the rating to a variable depending on the object we're using
        float sliderValue;
        float sliderMax;
        if (vrEnabled)
        {
            sliderValue = hud.confidenceSlider.GetComponent<LM_vrSlider>().sliderValue;
            sliderMax = hud.confidenceSlider.GetComponent<LM_vrSlider>().maxValue;
        } else
        {
            sliderValue = hud.confidenceSlider.GetComponent<Slider>().value;
            sliderMax = hud.confidenceSlider.GetComponent<Slider>().maxValue;
        }


        // -----------------------
        // Log Trial info
        // -----------------------

        // Get the parent and grandparent task to provide context in log file
        var parent = this.parentTask;
        var masterTask = parent;
        while (!masterTask.gameObject.CompareTag("Task"))
        {
            Debug.Log(masterTask.name);
            masterTask = masterTask.parentTask;
        }
        var rt = Time.time - startTime;
        // Output log for this task in tab delimited format
        log.log("LM_OUTPUT\tMentalNavigation.cs\t" + masterTask.name + "\t" + this.name + "\n" +
                "Task\tBlock\tTrial\tTargetName\tRating\tMaxRating\tRT\n" +
                masterTask.name + "\t" + masterTask.repeatCount + "\t" + parent.repeatCount + "\t" + objects.currentObject().name + "\t" + sliderValue + "\t" + sliderMax + "\t" + rt
                , 1);

        if (trialLog.active)
        {
            trialLog.AddData(transform.name + "_rating", sliderValue.ToString());
            trialLog.AddData(transform.name + "_rt", rt.ToString());
        }


        if (canIncrementLists) {

            if (objects) {
                objects.incrementCurrent ();
                currentObject = objects.currentObject ();
            }
            if (texts) {
                texts.incrementCurrent ();        
                currentText = texts.currentString ();
            }

        }

        GameObject avatar = manager.player.GetComponent<HUD>().Canvas as GameObject;
        Text canvas = avatar.GetComponent<Text>();
        string nullstring = null;
        canvas.text = nullstring;
//            StartCoroutine(storesInactive());

        if (actionButtonOn)
        {
            // Reset and deactivate action button
            actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;
            hud.actionButton.GetComponent<Button>().onClick.RemoveListener(hud.OnActionClick);
            hud.actionButton.SetActive(false);

            // make the cursor invisible
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        // Deactivate the slider
        hud.confidenceSlider.SetActive(false);

        // If we turned movement off; turn it back on
        if (restrictMovement)
        {
            manager.player.GetComponent<CharacterController>().enabled = true;
            manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = false;
        }
    }

}
