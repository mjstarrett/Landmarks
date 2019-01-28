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

var speed = 6.0;

private var controller : CharacterController;

public var LinearAccel : float;
public var LinearSpeed : float;
public var TurningAccel : float;
public var TurningSpeed : float;


	private var bootstrapped : System.Boolean;
	public var useAcceleration = false;
/*

         self.setMaxForwardSpeed(config['fullForwardSpeed'])
            self.setMaxBackwardSpeed(config['fullBackwardSpeed'])
            self.setMaxTurningSpeed(config['fullTurningSpeed'])
            
                Log.getInstance().addType("MOVINGOBJECT_COLLISIONRADIUS", \
                                [('identifier',basestring), ('value',Log.number)])
    Log.getInstance().addType("MOVINGOBJECT_TRIGGERSCOLLISIONS", \
                                [('identifier',basestring), ('value',bool)])
    Log.getInstance().addType("MOVINGOBJECT_LINEARACCEL", \
                                [('identifier',basestring), ('value',Log.number)])
                                     Sets the forward/backward acceleration
        of the player in VR units/s^2.
        
    Log.getInstance().addType("MOVINGOBJECT_LINEARSPEED", \
                                [('identifier',basestring), ('value',Log.number)])
    Log.getInstance().addType("MOVINGOBJECT_MAXBACKWARDSPEED", \
                                [('identifier',basestring), ('value',Log.number)])
    Log.getInstance().addType("MOVINGOBJECT_MAXFORWARDSPEED", \
                                [('identifier',basestring), ('value',Log.number)])
    Log.getInstance().addType("MOVINGOBJECT_MAXTURNINGSPEED", \
                                [('identifier',basestring), ('value',Log.number)])
    Log.getInstance().addType("MOVINGOBJECT_TURNINGACCEL", \
                                [('identifier',basestring), ('value',Log.number)])
    Log.getInstance().addType("MOVINGOBJECT_TURNINGSPEED", \
                                [('identifier',basestring), ('value',Log.number)])
                                
                                        # How much time has elapsed since the last frame?
        dt = globalClock.getDt()

        # Adjust linear speed according to user input.
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

        # Simulate some friction.
        if not self.intermission and abs(avatar.getLinearSpeed()) > 0:
            # Simulate some friction.
            if avatar.getLinearSpeed() > 0:
                avatar.setLinearSpeed(avatar.getLinearSpeed() - dt*config['linearAcceleration']*config['friction'])
            else:
                avatar.setLinearSpeed(avatar.getLinearSpeed() + dt*config['linearAcceleration']*config['friction'])
        
        # Move avatar depending on its current speed.
        avatar.move(dt)
        
        
            
            */
            
public var fullForwardSpeed : float;
public var fullBackwardSpeed : float;      
public var fullTurningSpeed : float;
        
function Start()
{
	controller = GetComponent(CharacterController);
	//bootstrapped = Config.instance.bootstrapped;
}

function Update() 
{

	moveDirection = Vector3(Input.GetAxis("FPShorizontal"), 0,
	                        Input.GetAxis("FPSvertical"));
	moveDirection = transform.TransformDirection(moveDirection);
	moveDirection *= speed;
	
	//MJS 07/15/2016 - removed jump function
//	       if (Input.GetButton ("Jump")) {
//	           moveDirection.y = 20;
//	       }
	
	var gravity = 200;
	// Apply gravity
	moveDirection.y -= gravity * Time.deltaTime;
	
	// Move the controller
	controller.Move(moveDirection * Time.deltaTime);
}


@script RequireComponent(CharacterController)