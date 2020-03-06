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
using UnityStandardAssets.Characters.ThirdPerson;

public enum RotationAxis
{
    X,
    Y,
    Z
}

public class ViewTargets : ExperimentTask {

    [Header("Task-specific Properties")]

    public ObjectList startObjects;
	private GameObject current;
	[HideInInspector] public GameObject destination;

	private static Vector3 position;
	private static Quaternion rotation;
	private static Vector3 scale;
	private int saveLayer;
	private int viewLayer = 11;
	public bool blackout = true;
	public bool rotate = true;
	public long rotation_start;
    public float rotation_start_float;
    public Vector3 objectRotationOffset;
    public RotationAxis rotationAxis;
    public float rotationSpeed = 30.0f;
    public bool restrictMovement = true;


    private Vector3 initialHUDposition;

    public override void startTask () {
		TASK_START();
		current = startObjects.currentObject();
		
		initCurrent();	
		rotation_start = Experiment.Now();
		rotation_start_float = rotation_start/1f;
		// Debug.Log(rotation_start);
		
	}	

	public override void TASK_START()
	{


		if (!manager) Start();
		base.startTask();
		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}
		

	    if (blackout) hud.showOnlyTargets();
	    else hud.showEverything();

        if (restrictMovement)
        {
            manager.player.GetComponent<CharacterController>().enabled = false;
            manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = true;
        }

        destination = avatar.GetComponentInChildren<LM_SnapPoint>().gameObject;

        // handle changes to the hud
        if (vrEnabled)
        {
            initialHUDposition = hud.hudPanel.transform.position;

            var temp = destination.transform.position;
            temp.y += 2.5f;
            hud.hudPanel.transform.position = temp;

        }
        else
        {
            // Change the anchor points to put the message at the bottom
            RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
            hudposition.pivot = new Vector2(0.5f, 0.1f);
        }
        

        // turn off all objects
        foreach (GameObject item in startObjects.objects)
        {
            item.SetActive(false);
        }
    }
	
	public override bool updateTask () {
		
		if (skip) {
			//log.log("INFO	skip task	" + name,1 );
			return true;
		}
		
		if (current) {
			
			if (rotate) {
                if (rotationAxis == RotationAxis.X)
                {
                    current.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
                }
                else if (rotationAxis == RotationAxis.Y)
                {
                    current.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }
                else if (rotationAxis == RotationAxis.Z)
                {
                    current.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
                }
                //log.log("TASK_ROTATE\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.localEulerAngles.ToString("f1"),2);
			}

			if ( Experiment.Now() - rotation_start >= interval)  {
				rotation_start = Experiment.Now();
				rotation_start_float = rotation_start/1f;
				startObjects.incrementCurrent();
				returnCurrent();
				current = startObjects.currentObject();	
				if (current) initCurrent();
			} else {
		    	return false;
			}
		} else {
			return true;
		}
		return false;
	}
	public void initCurrent() {
		position = current.transform.position;
        rotation = current.transform.rotation;
		scale = current.transform.localScale;
		
		current.transform.position = destination.transform.position;
		current.transform.rotation = destination.transform.rotation;
        current.transform.eulerAngles += objectRotationOffset;
		current.transform.localScale = destination.transform.localScale;

        // but Turn on the current object
        current.SetActive(true);

        saveLayer = current.layer;
		setLayer(current.transform,viewLayer);
        hud.setMessage(current.name);
        hud.ForceShowMessage();
		
		log.log("Practice\t" + current.name,1);

	}
	
	public override void TASK_ADD(GameObject go, string txt) {
		if (txt == "add") {
			saveLayer = go.layer;
			setLayer(go.transform,viewLayer);
		} else if (txt == "remove") {
			setLayer(go.transform,saveLayer);
		}

	}
	
	public void returnCurrent() {
		current.transform.position = position;
		current.transform.localRotation = rotation;
		current.transform.localScale = scale;
		setLayer(current.transform,saveLayer);

        // turn off the object we're returning before turning on the next one
        current.SetActive(false);
    }
	public override void endTask() {
		//returnCurrent();
		startObjects.current = 0;
		TASK_END();
	}
	
	public override void TASK_END() {

		base.endTask();

        if (vrEnabled)
        {
            hud.hudPanel.transform.position = initialHUDposition;
        }
        else
        {
            // Change the anchor points to put the message back in center
            RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
            hudposition.pivot = new Vector2(0.5f, 0.5f);
        }

        // turn on all targets
        foreach (GameObject item in startObjects.objects)
        {
            item.SetActive(true);
        }
    }

    public void setLayer(Transform t, int l) {
		t.gameObject.layer = l;
		foreach (Transform child in t) {
			setLayer(child,l);
		}
	}
}
