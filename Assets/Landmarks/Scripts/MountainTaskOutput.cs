using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class MountainTaskOutput : MonoBehaviour
{
    //Editor Variables for File(s)
    [Header("Output")]
    public bool generateOutput; //do we want to create a file
    public string inputName;    // what the file name is
    public bool textFile, excelFile; //do we want .txt or .xls
   
    //Editor Variables for Tracking of Head & Feet :=> Could be expanded to hands, and other trackers
    [Header("Tracking")]
    public GameObject head; //participants head
    public bool trackHead;  //do we want to track the participants head
    public GameObject leftFoot; //participants left foot
    public GameObject rightFoot;    //participants right foot
    public bool trackFeet;  //do we want to track participants feet
    
    //Scripting Variables for file(s) and tracking
    private string fileNames; //names given in editor
    private string textPath, excelPath; //names of the .txt and .xls file paths
    private GameObject[] targets;   //array of the target objects at the start of the experiment
    private int currentTargetNumber;    //the number representing the current target when the experiment is running
    private string currentTargetName;   //the name of the current target when the experiment is running
    private bool setNameNumber; //have we set the name & number of the target
    private int currentButton, previousButton; //boolean integers representing the button press of the current button press status and the previous button press status
    public static bool wasItPushed; //was the button pressed
    
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 90;

        Debug.Log("This is where we error check and validate our file names");
        //if the researcher wants output to be generated but both file types are unselected
        if (generateOutput && (!textFile && !excelFile))
        {
            //then we will throw an error letting the researcher know
                /// uncomment this for build and remove below two lines -> throw new System.Exception("There are no output files being generated despite Generate Output being selected");
            Debug.Log("There are no output files being generated despite Generate Output being selected");
            //and quit the experiment before it proceeds
           // UnityEditor.EditorApplication.isPlaying = false;
        }
        //however, if one or both of the file types are selected
        else if ((generateOutput && textFile) || (generateOutput && excelFile))
        {
            //then we will move on to path validation
            ValidatePath();
            //and formatting of the file
            FormatFile();
        }

        Debug.Log("This is where we validate the head & feet objects");
        //if the researcher wants to track the participants head and the object representing the participants head is nonexistant
        if (trackHead && head == null)
        {
            //then we throw an error letting the researcher know
            Debug.Log("Head tracking is enabled, but no head object is given");
            //and quit the experient before it proceeds
           // UnityEditor.EditorApplication.isPlaying = false;
        }

        //if the researcher wants to track the participants feet and the one or both of the objects representing the participants feet is/are nonexistant
        if (trackFeet && (leftFoot == null || rightFoot == null))
        {
            //then we throw an error letting the researcher know
            Debug.Log("Foot tracking is enabled, but one or more feet objects are missing");
            //and quit the experiment before it proceeds
          //  UnityEditor.EditorApplication.isPlaying = false;
        }

        //if the researcher has not selected the track head and track feet objects
        if (!trackHead && !trackFeet)
        {
            //we let them know, but proceed because this is still a valid experiment
            Debug.Log("CustomError: Tracking is disabled for BOTH head and feet");
        }


        //Populate targets[] with the target objects
        PopulateTargetArray();

        //Initialize the current button push state to be 0 (false)
        currentButton = 0;
        //Initialize the boolean representing if the button was pushed to be false
        wasItPushed = false;

        StartCoroutine(Logging());

    }//End of Start

    // FixedUpdate is called once every time step
        //Make sure Fixed Timestep = .0200000 to get an output of always 20ms
            //Fixed Timestep is located in the Time settings (Edit > Project Settings > Time > Fixed Timestep) or in TimeManager.asset (ProjectName > ProjectSettings > TimeManager.asset)
     IEnumerator Logging()
    {
        while (true)
        {
            if (GreekNavigationTask.trialNumber > 0)
            {
                //Validate the button press state
                ValidateButtonPress();
                //Set the target data values
                SetTargetData();
                //Write all the apropriate data to the file(s) (except on the break instruction (which should be instruction task #101)
                if (InstructionsTask.instructionsCounter != 101)
                {
                    WriteToFile();
                }

                //Set the previous button push state to the current
                previousButton = currentButton;
            }
            yield return new WaitForSeconds(0.03000000f);
        }
    }//End of FixedUpdate

    //ValidatePath is called once in Start()
        //ValidatePath is used to make sure that the file(s) given are valid (i.e. they dont already exist)
    private void ValidatePath()
    {
        //set the file names to be equal to the given string (we don't directly check the given string for variable security reasons; i.e. we don't want to accidentally override it)
        fileNames = inputName;
        Debug.Log("Our file names are: " + fileNames);
        //if the fileNames is NOT an empty string
        if (fileNames != "")
        {
            //and we want a textfile for an output
            if (textFile)
            {
                //then we set the path to be our current directory and give the log.txt extension
                textPath = Application.dataPath + "/" + fileNames + "log.txt";
                Debug.Log("Our textPath is: " + textPath);
                //and if the file doesnt exist
                if (!File.Exists(textPath))
                {
                    //then we write to the file its name + Output File on the first line
                    File.WriteAllText(textPath, fileNames + " Output File \n");
                }
                //however, if the file exists
                else
                {
                    //then we will throw an error letting them know, and stop the experiment before it progresses
                        ///uncomment this for build and remove below two lines -> throw new System.Exception("One or more of the file names already exists");
                    Debug.Log("One or more of the file names already exists");
                 //   UnityEditor.EditorApplication.isPlaying = false;
                }
            }
            //and we want an excelfile for output
            if (excelFile)
            {
                //then we set the path to be our current directory and give the log.xls extension (NOTE: this is an older extension that excel may claim to be "corrupted." It is not and the newer extension doesn'w work properly with line breaks).
                excelPath = Application.dataPath + "/" + fileNames + "log.xls";
                Debug.Log("Our excelPath is: " + excelPath);
                //and if the file doesnt exist
                if (!File.Exists(excelPath))
                {
                    //then we write to the file its name + Output File on the first line
                    File.WriteAllText(excelPath, fileNames + " Output File \n");
                }
                //however, if the file exists
                else
                {
                    //then we will throw an error letting them know, and stop the experiment before it progresses
                        ///uncomment this for build and remove below two lines -> throw new System.Exception("One or more of the file names already exists");
                    Debug.Log("One or more of the file names already exists");
                 //   UnityEditor.EditorApplication.isPlaying = false;
                }
            }
            
        }
        //however, if the file name is an empty string
        else
        {
            //then we will throw an error letting them know, and stop the experiment before it progresses
                ///uncomment this for build and remove below two lines -> throw new System.Exception("Please give an output file name!");
            Debug.Log("Please give an output file name!");
         //   UnityEditor.EditorApplication.isPlaying = false;
        }
    }//End of ValidatePath

    //FormatFile is called once in Start()
        //FormatFile is used to setup the columns in the ouputfile
    private void FormatFile()
    {
        //if we are outputting a text file
        if (textFile)
        {
            //write the columns: Time, Trial, TargetName, and TargetNumber to the txt file
            File.AppendAllText(textPath, "Time \tTrial \tTargetName \tTargetNumber");
            //if we are tracking the participants head
            if (trackHead)
            {
                //write the columns: HeadX, HeadZ, and HeadAngle to the txt file
                File.AppendAllText(textPath, "\tHeadX \tHeadZ \tHeadAngle");
            }
            //if we are tracking the participants feet
            if (trackFeet)
            {
                //write the columns: LFootX, LFootZ, LFootAngle, RFootX, RFootZ, and RFootAngle to the txt file
                File.AppendAllText(textPath, "\tLFootX \tLFootZ \tLFootAngle \tRFootX \tRFootZ \tRFootAngle");
            }
            //write the last column: IsPredictedLocation to the txt file
            File.AppendAllText(textPath, "\tIsPredictedLocation\n");
        }

        //if we are outputting an excel file
        if (excelFile)
        {
            //write the columns: Time, Trial, TargetName, and TargetNumber to the xls file
            File.AppendAllText(excelPath, "Time \tTrial \tTargetName \tTargetNumber");
            //if we are tracking the participants head
            if (trackHead)
            {
                //write the columns: HeadX, HeadZ, and HeadAngle to the xls file
                File.AppendAllText(excelPath, "\tHeadX \tHeadZ \tHeadAngle");
            }
            //if we are tracking the participants feet
            if (trackFeet)
            {
                //write the columns: LFootX, LFootZ, LFootAngle, RFootX, RFootZ, and RFootAngle to the xls file
                File.AppendAllText(excelPath, "\tLFootX \tLFootZ \tLFootAngle \tRFootX \tRFootZ \tRFootAngle");
            }
            //write the last column: IsPredictedLocation to the xls file
            File.AppendAllText(excelPath, "\tIsPredictedLocation\n");
        }
    }//End of FormatFile

    //ValidateButton is called at the start of FixedUpdate() and gets called every timestep (20ms)
        //ValidateButton is used to ensure that the button press prediciton is only written to the file once per trial
    private void ValidateButtonPress()
    {
        //set the current button press state to be equal to the prediction value in GreekNavigationTask
        currentButton = GreekNavigationTask.prediction;
        //if the previous button press state is 1 (true) and the current button press state is 1 (true)
        if (previousButton == 1 && currentButton == 1)
        {
            //then set the current button press state to be 0
            currentButton = 0;
            //and set the button pressed boolean to true
            wasItPushed = true;
        }
        //if the button pressed boolean is true
        if (wasItPushed)
        {
            //then we set the button press state to be 0 (false)
            currentButton = 0;
        }
    }//End of ValidateButton

    //WriteToFile is called in FixedUpdate() after SetTargetData()
        //WriteToFile is used to write all of the pertinent data to the pertinent file(s)
    private void WriteToFile()
    {
        //if we are outputting a text file
        if (textFile)
        {
            //if the GreekNavigationTask is currently running
            if (GreekNavigationTask.running)
            {
                //Write the Time, Trial#, TargetName, and TargetNumber to the txt file
                //                               Time in ms                            Trial #                         TargetName                  TargetNumber
                File.AppendAllText(textPath, ((Time.time * 1000) - Time.deltaTime) + "\t" + GreekNavigationTask.trialNumber + "\t" + currentTargetName + "\t" + currentTargetNumber);
            }
            //however, if GreekNavigationTask is not currently running
            else if (GreekNavigationTask.running == false)
            {
                //Write the Time, Inverse Trial#, No Target Name, and No Target Name to the txt file
                //                               Time in ms                           Trial #                        TargetName     TargetNumber
                File.AppendAllText(textPath, ((Time.time * 1000) - Time.deltaTime) + "\t" + (-(GreekNavigationTask.trialNumber)) + "\t" + "None" + "\t" + "None");
            }

            //if we are tracking the participants head
            if (trackHead)
            {
                //Write the x & z coordinates and y angle of the participants head to the txt file
                //                              head x position         head z position         head angle
                File.AppendAllText(textPath, "\t" + GetX(head) + "\t" + GetZ(head) + "\t" + GetAngle(head));
            }
            //if we are tracking the participants feet
            if (trackFeet)
            {
                //Write the x & z coordinates and y angle of the participants left and right feet to the txt file
                //                              left foot x position        left foot z position    left foot angle         right foot x position     right foot z position         right foot angle
                File.AppendAllText(textPath, "\t" + GetX(leftFoot) + "\t" + GetZ(leftFoot) + "\t" + GetAngle(leftFoot) + "\t" + GetX(rightFoot) + "\t" + GetZ(rightFoot) + "\t" + GetAngle(rightFoot));
            }
            //Write the current button press value to the txt file
            File.AppendAllText(textPath, "\t" + currentButton + "\n");
        }

        //if we are outputting an excel file
        if (excelFile)
        {
            //if the GreekNavigationTask is currently running
            if (GreekNavigationTask.running)
            {
                //Write the Time, Trial#, TargetName, and TargetNumber to the xls file
                //                               Time in ms                            Trial #                         TargetName                  TargetNumber
                File.AppendAllText(excelPath, ((Time.time * 1000) - Time.deltaTime) + "\t" + GreekNavigationTask.trialNumber + "\t" + currentTargetName + "\t" + currentTargetNumber);
            }
            //however, if GreekNavigationTask is not currently running
            else if (GreekNavigationTask.running == false)
            {
                //Write the Time, Inverse Trial#, No Target Name, and No Target Name to the xls file
                //                               Time in ms                            Trial #                       TargetName       TargetNumber
                File.AppendAllText(excelPath, ((Time.time * 1000) - Time.deltaTime) + "\t" + (-(GreekNavigationTask.trialNumber)) + "\t" + "None" + "\t" + "None");
            }

            //if we are tracking the participants head
            if (trackHead)
            {
                //Write the x & z coordinates and y angle of the participants head to the xls file
                //                              head x position         head z position         head angle
                File.AppendAllText(excelPath, "\t" + GetX(head) + "\t" + GetZ(head) + "\t" + GetAngle(head));
            }
            //if we are tracking the participants feet
            if (trackFeet)
            {
                //Write the x & z coordinates and y angle of the participants left and right feet to the xls file
                //                              left foot x position        left foot z position    left foot angle         right foot x position     right foot z position         right foot angle
                File.AppendAllText(excelPath, "\t" + GetX(leftFoot) + "\t" + GetZ(leftFoot) + "\t" + GetAngle(leftFoot) + "\t" + GetX(rightFoot) + "\t" + GetZ(rightFoot) + "\t" + GetAngle(rightFoot));
            }
            //Write the current button press value to the txt file
            File.AppendAllText(excelPath, "\t" + currentButton + "\n");
        }
    }//End of WriteToFile

    //SetTargetData is called in FixedUpdate() after ValidateButtonPress()
        //SetTargetData is used to establish the current target's name and corresponding number
    private void SetTargetData()
    {
        //If the GreekNavigationTask is running
        if (GreekNavigationTask.running)
        {
            //if we havent set the target name and number
            if (!setNameNumber)
            {
                //set the current targets number by calling the GetCurrentTargetNumber() function
                currentTargetNumber = GetCurrentTargetNumber();
                //set the current targets name
                currentTargetName = GreekNavigationTask.targetName;
                Debug.Log("We are on trial: " + GreekNavigationTask.trialNumber + ", The current target is the: " + currentTargetName + ", whose number is: " + currentTargetNumber);
                //set the boolean value to true
                setNameNumber = true;
            }
        }
        //but, if the GreekNavigationTask is not running
        else
        {
            //set the boolean value to false
            setNameNumber = false;
        }
    }//End of SetTargetData

    //PopulateTargetArray is called at the end of Start()
        //PopulateTargetArray is used to initialzie an array containing the target objects
    private void PopulateTargetArray()
    {
        //create a new array of GameObjects with a length of the amount of targets
        targets = new GameObject[GameObject.Find("TargetObjects").transform.childCount];
        //sort the Array
        Array.Sort(targets);
        //initialize a counter
        int i = 0;
        //iterate through every child in TargetObjects with a transform component
        foreach (Transform child in GameObject.Find("TargetObjects").transform)
        {
            //set the array to hold the child object at the index represented by the counter varaible
            targets[i] = child.gameObject;
            //increment the counter variable
            i++;
        }
    }//End of PopulateTargetArray

    //GetCurrentTargetNumber is called in SetTargetData
        //GetCurrentTargetNumber is used to compare the current target name to every target name in the targets array and return the appropriate number of said target
    private int GetCurrentTargetNumber()
    {
        //initialize a counter
        int j = 1;
        //iterate through every object in the targets array
        foreach (GameObject t in targets)
        {
            //if the current target name is the same as the name of the object stored in targets
            if (t.name == GreekNavigationTask.targetName)
            {
                //then we return the appropriate number
                return j;
            }
            //if not then we increment the counter
            j++;
        }
        //We have to return a number at this point. If this value gets returned then something broke.
        return -64;
    }//End of GetCurrentTargetNumber

    //GetX is called in WriteToFile
        //GetX is used to shorten the code required to get the x position value of a supplied object
    private float GetX(GameObject obj)
    {
        //return the x position of a given object
        return obj.transform.position.x;
    }//End of GetX

    //GetZ is called in WriteToFile
        //GetZ is used to shorten the code required get the z position value of a supplied object
    private float GetZ(GameObject obj)
    {
        //return the z position of a given object
        return obj.transform.position.z;
    }//End of GetZ

    //GetAngle is called in WriteToFile
        //GetAngle is used to shorten the code required get the y angle value of a supplied object
    private float GetAngle(GameObject obj)
    {
        //return the y angle of a given object
        return obj.transform.eulerAngles.y;
    }//End of GetAngle

}//End of MountainTaskOutput : Monobehaviour
