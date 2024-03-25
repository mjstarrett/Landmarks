/*
    Copyright (C) 2010  Jason Laczko

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class MovePlayer : ExperimentTask {
    
    [HideInInspector] public GameObject start;

    [Header("Task-specific Properties")]
    public GameObject destination;
    public string destinationListName;
	public ObjectList destinations;

	public bool swap;

    public bool randomRotation;
	public bool scaledPlayer = false;
    public bool ignoreY = false;
    public bool useSnapPoint;
    public Vector3 localOffsetFacing;

    //variables used for block repetition
    public bool blockRepeat;
    public static int repetition;
    private int count = 1;

	public override void startTask () {
        // Debug.Log("MS Repetition:"+repetition);
        TASK_START();
	}

    public override void TASK_START()
    {
        base.startTask();

        if (!manager) Start();


        if (skip) {
            log.log("INFO	skip task	" + name, 1);
            return;
        }

        if (scaledPlayer)
        {
            start = scaledAvatar;
        } else start = avatar;
        // Debug.Log("Player identified: " + start.gameObject.name);

        // Find destinations with string destinationsName
        if (destinations == null && destinationListName != "") // only if destinations is blank and destinationsName is not
        {
            destinations = GameObject.Find(destinationListName).GetComponent<ObjectList>();
            // Debug.Log("moving the " + start.gameObject.name + " to " + destinations.currentObject());
        }
        // otherwise, use destination or destinations.
        if (destinations) {
            destination = destinations.currentObject();
            //Debug.Log("Destination selected: " + destination.name +
            //    " (" + destination.transform.position.x + ", " +
            //    destination.transform.position.z + ")");
        }

        // Temporary transforms that can be easily modified
        var originPos = start.transform.position;
        var originRot = start.transform.eulerAngles;
        var terminusPos = useSnapPoint ?    destination.transform.GetComponentInChildren<LM_SnapPoint>().transform.position : 
                                            destination.transform.position;
        Debug.Log(terminusPos.y);
        var terminusRot = useSnapPoint ?    destination.transform.GetComponentInChildren<LM_SnapPoint>().transform.localEulerAngles :
                                            destination.transform.eulerAngles;

        // -----------------
        // Move the player
        // -----------------
        // Character controller component must be disabled to move the player like this
        if (localOffsetFacing != Vector3.zero) terminusPos += destination.transform.TransformDirection(localOffsetFacing);
        if (ignoreY) terminusPos.y = originPos.y;
        start.GetComponentInChildren<CharacterController>().enabled = false;
        start.transform.position = terminusPos;
        log.log("TASK_POSITION\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.transform.position.ToString("f1"), 1);
        start.GetComponentInChildren<CharacterController>().enabled = true;

        // -----------------
        // Rotate the player
        // -----------------
        // Turn off any FirstPersonController (FPC) components in order to modify
        var fpc = start.GetComponent<FirstPersonController>();
        if (fpc != null) fpc.enabled = false;
        // Modify and/or set
        if (localOffsetFacing != Vector3.zero)
        {
            Debug.LogWarning("Using the provided 'localOffsetFacing' property to point the player at the original destination.\n" +
                "\tIf 'randomRotation' was selected, it will be ignored.");
            start.transform.LookAt(destination.transform);
            // level off the viewpoint
            start.transform.eulerAngles = new Vector3(0f, start.transform.eulerAngles.y, 0f);
        }
        else
        {
            if (randomRotation) terminusRot.y = Random.Range(0, 360 - Mathf.Epsilon);
            start.transform.eulerAngles = terminusRot;
        }
        manager.playerCamera.transform.localEulerAngles = Vector3.zero;
        // turn the FPC back on
        if (fpc != null) fpc.ResetMouselook();
        if (fpc != null) fpc.enabled = true;
        
        

        if (!isScaled)
        {
            /* MJS 2019
             * It is not possible to simply rotate the unity standard asset firstpersoncontroller manually,
             * so we need to access a modified firstpersoncontroller.cs script
             * which will access a modified MouseLook.cs script to reset the mouselook
             * which effectively forces it not to undo our manual rotation
            */
            //avatar.GetComponent<FirstPersonController>().ResetMouselook();
        }

        log.log("TASK_ROTATE\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.localEulerAngles.ToString("f1"), 1);


        //Debug.Log("-------------------------------------------------");
        //Debug.Log(start.transform.position);
        //Debug.Log(destination.transform.position);
        //Debug.Log(avatar.transform.position);
        //Debug.Log("-------------------------------------------------");

        if (swap)
        {
            destination.transform.position = originPos;
            destination.transform.eulerAngles = originRot;
        }

        Debug.Log("Player at (" + manager.player.transform.position.x + ", " + manager.player.transform.position.y + ", " + manager.player.transform.position.z + ") and facing " + manager.player.transform.eulerAngles.y + "°");
	}

	public override bool updateTask () {
	    return true;
	}
	public override void endTask() {




		TASK_END();
	}

	public override void TASK_END() {
		base.endTask();
		if ( destinations ) {
            if (blockRepeat)
            {
                BlockIncrementation();
            }
            else
            {
                if (canIncrementLists) destinations.incrementCurrent();
            }
            destination = destinations.currentObject();
		}
	}

    //Method to get implemented when the Block Increment boolean in the GUI is selected
    public void BlockIncrementation()
    {
        //Debug.Log("count before increment: " + count);

        //If the player has done # of repetitions equal to the parent task Repetition Value then set count back to 0 & move the Spawn Location to the next in the list
        if (count == repetition)
        {
            count = 1;
            if (canIncrementLists) destinations.incrementCurrent();
        }
        else //increment count
        {
            count++;
        }
        //Debug.Log("count after increment: " + count);
    }
}
