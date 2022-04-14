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
using System;


public class AvatarController : MonoBehaviour {
	
	private GameObject experiment;
	private dbLog log;
	private Experiment manager;
	private Transform avatar;
	public Transform mainCamera;
	private CharacterController controller;
	
	public float sensitivityX = 9F;
	public float sensitivityY = 7F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	public float moveSensitivity = 1.0f;
	
	float rotationX = 0F;
	float rotationY = 0F;
	
	public float speed = 20.0F;
	
	Quaternion originalRotation;
	
	private bool bootstrapped;
	public bool debugAcceleration = false;
	
	public float movefriction           = 0.8F;
	public float linearAcceleration = 5.0F;
	public float fullForwardSpeed   = 2.0F;
	public float fullBackwardSpeed  = -2.0F;
	
	public float fullTurningSpeed   = 60F;
	public float turningAcceleration = 60F;
	
	public float linearSpeed  = 0.0F;
	public float turningSpeed = 0.0F;	
	
	public float target = 0.0F;
	public float vel = 0.0F;
	public float rot = 0.0F;
	public float rotTime = 0.5F;
	public float rotDamp = 0.5F;
	public float maxRotVel = 10F;
	public float movementInput = 0F;
	public float fixedTarget = 1F;
	public float fixedrot = 1F;
	public float fixedrotm = 1.8F;
	
	
	[HideInInspector] public bool handleInput = false;
	
	
	/*
	1308888313144	0	INPUT_EVENT	turnLeft	1
1308888313144	0	INPUT_EVENT	moveForward	1
1308888313145	0	MOVINGOBJECT_LINEARSPEED	PandaEPL_avatar	2.203068
1308888313145	0	MOVINGOBJECT_TURNINGSPEED	PandaEPL_avatar	1.00458
1308888313145	0	MOVINGOBJECT_LINEARSPEED	PandaEPL_avatar	2.136096
1308888313146	1	VROBJECT_HEADING	PandaEPL_avatar	171.360508648
1308888313146	1	VROBJECT_POS	PandaEPL_avatar	Point3(-49.6813, 8.3998, 1)
1308888313146	1	VROBJECT_HPR	PandaEPL_defaultCamera	VBase3(171.361, 0, 0)
*/
	
	public void AVATAR_HPR(Vector3 hpr) {
		avatar.localEulerAngles = hpr;
	}
	
	public void AVATAR_POS(Vector3 pos) {
		avatar.position = pos;
	}
	
	public void stop() {
		//someday make these be real speed values
		Vector3 vec = new Vector3(0.0F, 0.0F, 0.0F);
		
		AVATAR_STOP(vec);
		log.log("AVATAR_STOP	" +  vec.ToString("f0"),1);
	}
	public void AVATAR_STOP(Vector3 vec) {
		linearSpeed = 0.0F;
		turningSpeed = 0.0F;
	}
	
	void FixedUpdate ()
		//void public handleInput() 
	{
		if (handleInput) {
			if (bootstrapped || debugAcceleration)
			{
				//	        	if (Input.GetAxis("Horizontal") < 0.05F && Input.GetAxis("Horizontal") > -0.05F) {
				//	        		target = 0.0F;
				//	        		rot = 0.0F;
				//	        		fixedrot = 0.0F;
				//	        	} else {
				//	        		target += Input.GetAxis("Horizontal") * rotDamp; // * fullTurningSpeed;
				//	        		target = fixedTarget;
				//	        		vel = 2.0F;
				//	        		fixedrot = fixedrotm * Input.GetAxis("Horizontal");
				//	        	}
				if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.RightArrow)){
					if (Input.GetKey (KeyCode.LeftArrow)){
						target += -1* rotDamp; // * fullTurningSpeed;
						target = fixedTarget;
						fixedrot = fixedrotm * -1;
						vel = 2.0F;
					}else{
						target += 1* rotDamp; // * fullTurningSpeed;
						target = fixedTarget;
						vel = 2.0F;
						fixedrot = fixedrotm * 1;
					}
				}else{
					
					target = 0.0F;
					rot = 0.0F;
					fixedrot = 0.0F;
					
				}
				rot = Mathf.SmoothDamp(fixedrot, target, ref vel, rotTime, maxRotVel);
				float rotateInput = Input.GetAxis("Horizontal") * Time.deltaTime * fullTurningSpeed;
				//float movementInput = Input.GetAxis("Vertical");
				if (Input.GetKey (KeyCode.UpArrow)||Input.GetKey (KeyCode.UpArrow)){
					if (Input.GetKey (KeyCode.UpArrow)){
						movementInput = 1;
					}else{
						movementInput = -1;
					}
				}else{
					movementInput = 0;
				}
				
				
				changeAvatar(movementInput, rot);
				
				if ( rotateInput != 0.0F ) {
					log.log("INPUT_EVENT	turn	" + rotateInput,2 );
					log.log("AVATAR_HPR	" + avatar.localEulerAngles.ToString("f3"),2);
				}
				if ( movementInput != 0.0F ) {
					log.log("INPUT_EVENT	move	" + movementInput,2 );
				}
				if (linearSpeed != 0.0F) {
					log.log("AVATAR_POS	" + avatar.position.ToString("f3"),2);
				}
				
			} else {
				// Read the mouse input axis
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				
				rotationX = ClampAngle(rotationX, minimumX, maximumX);
				rotationY = ClampAngle(rotationY, minimumY, maximumY);
				
				Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
				Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
				
				mainCamera.localRotation = originalRotation * yQuaternion;
				avatar.localRotation = originalRotation * xQuaternion;
				
				//Vector3 moveDirection = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
				Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				moveDirection = mainCamera.TransformDirection(moveDirection);
				moveDirection.x *= speed;
				moveDirection.z *= speed;
				
				var gravity = 200;
				// Apply gravity
				moveDirection.y -= gravity * Time.deltaTime;
				
				// Move the controller
				controller.Move(moveDirection * Time.deltaTime);
			}
		}         
		//log.log("VROBJECT_POS	timerText	Point3(0.83, 0, 0.916667)", 1);
		//log.log("MOVINGOBJECT_LINEARSPEED	PandaEPL_avatar	0.24117", 1);
		
	}
	
	public void changeAvatar(float movementInput, float rotateInput) {
		// Adjust linear speed according to user input.                      
		linearSpeed = linearSpeed + (Time.deltaTime * movementInput * linearAcceleration);
		
		if (movementInput == 0.0 && Math.Abs(linearSpeed) < 0.1F) {
			linearSpeed = 0.0F;
		} 
		
		//friction
		float calcfric = (movefriction * Time.deltaTime * linearAcceleration);
		//Debug.Log(linearSpeed);
		if (linearSpeed > 0.0F) {
			linearSpeed = linearSpeed - calcfric;
			if (linearSpeed < 0.0F) linearSpeed = 0.0F;
		} else if (linearSpeed < 0.0F) {
			linearSpeed = linearSpeed + calcfric;
			if (linearSpeed > 0.0F) linearSpeed = 0.0F;
		}
		
		
		if (linearSpeed > fullForwardSpeed) {
			linearSpeed = fullForwardSpeed;//   * Time.deltaTime;
		}
		if (linearSpeed < fullBackwardSpeed) {
			linearSpeed = fullBackwardSpeed;// * Time.deltaTime;
		} 
		
		
		// Move the controller
		Vector3 moveDirection = new Vector3(0.0F, 0.0F, 1.0F);
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= linearSpeed;
		controller.Move(moveDirection); // * Time.deltaTime);
		
		//Debug.Log(Input.GetAxis("Horizontal"));
		avatar.Rotate(Vector3.up * rotateInput);
		
	}
	
	void Start ()
	{
		//controller = avatar.GetComponent("Character Controller") as CharacterController;
		
		//controller = avatar.GetComponent<CharacterController>();
		controller = gameObject.GetComponent<CharacterController>();
		avatar = transform;
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		originalRotation = avatar.localRotation;
		
		bootstrapped = Config.Instance.bootstrapped;
		
		experiment = GameObject.FindWithTag ("Experiment");
		manager = experiment.GetComponent("Experiment") as Experiment;
		log = manager.dblog;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
}

/*
      if self.inputEvents.has_key('moveForward') and not self.intermission:
            # Acceleration is scaled by how hard the
            # user is pressing the forward key.
            avatar.setLinearAccel(config['linearAcceleration'] * \
                                      self.inputEvents['moveForward'].getMagnitude())
                                      
                                                  # Update instantaneous speed.
            newSpeed = avatar.getLinearSpeed() + dt*avatar.getLinearAccel()
            avatar.setLinearSpeed(newSpeed)
        if self.inputEvents.has_key('moveBackward') and not self.intermission:
            # Acceleration is scaled by how hard the
            # user is pressing the backward key.
            avatar.setLinearAccel(config['linearAcceleration'] * \
                                      self.inputEvents['moveBackward'].getMagnitude())
            # Update instantaneous speed.
            newSpeed = avatar.getLinearSpeed() - dt*avatar.getLinearAccel()
            avatar.setLinearSpeed(newSpeed)
        # If speed is negligible, clamp it to zero.
        if not self.inputEvents.has_key('moveForward') and \
           not self.inputEvents.has_key('moveBackward') and \
           abs(avatar.getLinearSpeed()) < 0.1:
            avatar.setLinearSpeed(0)

        # Adjust turning speed according to user input.
        if self.inputEvents.has_key('turnLeft') and not self.intermission:
            # Acceleration is scaled by how hard the
            # user is pressing the left key.
            avatar.setTurningAccel(config['turningAcceleration'] * \
                                       self.inputEvents['turnLeft'].getMagnitude())
            # Update instantaneous speed.
            newSpeed = avatar.getTurningSpeed() + dt*avatar.getTurningAccel()
            avatar.setTurningSpeed(newSpeed)
        if self.inputEvents.has_key('turnRight') and not self.intermission:
            # Acceleration is scaled by how hard the
            # user is pressing the right key.
            avatar.setTurningAccel(config['turningAcceleration'] * \
                                       self.inputEvents['turnRight'].getMagnitude())
            # Update instantaneous speed.
            newSpeed = avatar.getTurningSpeed() - dt*avatar.getTurningAccel()
            avatar.setTurningSpeed(newSpeed)
        # If user lets go of respective input keys, stop turning right away.
        if not self.inputEvents.has_key('turnLeft') and not self.inputEvents.has_key('turnRight'):
            avatar.setTurningSpeed(0)
*/
/*
        # Simulate some friction.
        if not self.intermission and abs(avatar.getLinearSpeed()) > 0:
            # Simulate some friction.
            if avatar.getLinearSpeed() > 0:
                avatar.setLinearSpeed(avatar.getLinearSpeed() - dt*config['linearAcceleration']*config['friction'])
            else:
                avatar.setLinearSpeed(avatar.getLinearSpeed() + dt*config['linearAcceleration']*config['friction'])
  */      
