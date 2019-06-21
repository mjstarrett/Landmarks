------------------------------------------------------------------------------
 Copyright © 2019 Tobii Pro AB. All rights reserved.
------------------------------------------------------------------------------

Points 1 - 2 are needed to enable Steam VR and Tobii eye tracking in a project. 

1. Import the SteamVR package from the asset store.
2. Import the TobiiPro.SDK.Unity.Windows package.

Points 3 - 6 show how to enable Steam VR in a scene

3. Remove any camera in the scene (the default camera is called "Main Camera").
   When creating a scene from scratch and importing Steam VR, there will be a
   conflict with the default camera in the scene.
4. Drag and drop the following prefabs into the scene:
    "SteamVR\Prefabs\[CameraRig]"
    "SteamVR\Prefabs\[SteamVR]"
5. Click the object "[CameraRig]\Controller (right)" GameObject and Add the
   component SteamVR_TrackedController
6. Click the object "[CameraRig]\Controller (left)" GameObject and Add the
   component SteamVR_TrackedController

Note that the interaction with the tracked controllers in the TobiiControl.cs
script uses a reflection based solution in order to not have a hard dependency
on the SteamVR package code. For additional development it is recommended to use
the standard SteamVR techniques as described in the SteamVR documentation.


Points 7 - 12 show how to enable Tobii eye tracking using the TobiiControl
package and show the gaze point on an object.

7. Drag and drop the prefab "TobiiPro\Examples\VRDemo\Prefabs\TobiiControl"
   into the scene.
8. Place an object in the scene, such as a cube, and make sure it has a
   collider attached.
9. Drag the "TobiiPro\VR\Prefabs\[VREyeTracker]" prefab into the scene. Make
   sure the "Subscribe To Gaze" check box is enabled.
10. Drag the "TobiiPro\VR\Prefabs\[VRCalibration]" prefab into the scene.
11. Drag the "TobiiPro\VR\Prefabs\[VRGazeTrail]" prefab into the scene. Change
    the "Particle Count" to 1. Make sure "On" is checked.
12. Drag the "TobiiPro\VR\Prefabs\[VRSaveData]" prefab into the scene. Select
    which kind of data that should be saved by checking "Save Unity Data" and/or
    "Save Raw Data".

    13. Play the game. Follow the instructions on the sign in the scene. Looking at
    the object should place the gaze point on it. The tracking data for each
    session is stored in Data folder in the root folder of the application.

--

There are two example scenes that can be opened to see how to use the package:
CalibrationExample and InteractionExample.

CalibrationExample is a very basic scene. A simple room, with nothing on it.
The gaze point will collide with the walls, floor and roof.

InteractionExample is a more advanced example in which the objects in the scene
(cubes/cylinders) change colour when the user looks at them.
