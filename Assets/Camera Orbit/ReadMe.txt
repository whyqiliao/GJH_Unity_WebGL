Thanks for purchase Orbit Camera.

- Get Started:

- Import the 'Orbit Camera' in your project.
- Drag the prefab 'Orbit Camera' (Camera Orbit/Content/Prefabs/Orbit Camera) to the scene that you want add.
- Drag the target to camera orbit to the var 'Target' of bl_CameraOrbit' of Orbit Camera prefab in scene.
- Customize variables to your taste and you are ready!.

---------------------------------------------------------------------------------------------------------------

Add button for rotate: 

- When you need rotate camera with UI buttons, you only need do this:
   
   - Create the Buttons (Horizontals and Verticals as you need)
   - in this buttons add the script bl_OrbitButton.cs
   - in the var "Camera Orbit" set the camera target to rotate (with a bl_CameraOrbit script attached on the gameObject).
   - Setting the directional Axis that you need rotate.
   - Where amount of Horizontal is < 0 = Right, > 0 is Left and in Vertical < 0 is down , > 0 is up.
   - Ready!.

-------------------------------------------------------------------------------------------------------------

Use For Mobile.
-If you want use for mobile, you only need setup a few things for this:
first, in bl_CameraOrbit script mark the field "isForMobile" as true.

From version 1.5++:
You can use bl_OrbitTouch.cs script instead of bl_OrbitTouchPad, the difference is that bl_OrbitTouch doesn't require an UI Image to detect the inputs
the advantage of bl_OrbitTouchPad is that you can easy setup the are where can detect the input touch (with the UI Image size) and avoid problems with other UI.

For use bl_OrbitTouchPad:

create a UI Image (UGUI) in your canvas and add the script "bl_OrbitTouchPad", remember that the area of this image,
is the are where you can use for rotate the camera, so is recommendable that you set this in the bottom of all UI for not block others UI intractable.

- else if you use the "bl_OrbitTouchPad" remember set the field "OverrideEditor" as true for avoid wrong movements.

-------------------------------------------------------------------------------------------------------------
Public Vars:

Transform Target:                       Transform to camera orbit and follow.
bool AutoTakeInfo:                      take the closest circular position to the default position (editor position) at the start.
float Distance:                         Distance between the camera and the target.
float SwichtSpeed:                      The speed with the camera will switch of target
Vector2 DistanceClamp:                  Min and Max Distance (x = min y = max).
Vector2 YLimitClamp:                    Min and Max Y angle limit (x = min y = max).
Vector2 XLimitClamp:                    Clamp the horizontal angle from the start position (max left = x, max right = y) >= 360 = not limit
bool LockCursorOnRotate:                Lock / Hide cursor when screen it's touched for rotate
Vector2 SpeedAxis:                      Speed of movement in respective angles (x = horizontal y = vertical)
bool RequieredInput:                    Required input for rotate and move camera or simple rotate depend of mouse axis?
CameraMouseInputType RotateInputKey:    Key to need be input to start rotate to the mouse / touch direction.
KeyCode InputKey:                       Key to move camera (when RequieredInput is true).
float InputMultiplier:                  how much multiple the input amount.
float InputLerp:                        Smooth for input value.
bool UseKey:                            use Horizontal and Vertical Key axis instead of mouse axis?
float PuwFogAmount:                     Amount for 'puw' effect of camera (zoom when up click)
float OutInputSpeed:                    Speed to apply to rotation when Input is up.
float FogStart:                         Where the fog camera will start?
float FogLerp:                          Smooth amount for fog transition.
float DelayStartFog:                    how much time take to fog go to normal fog from StartFog (just in start function).
float ScrollSensitivity:                How much increase the zoom in each scroll rotation
float ZoomSpeed:                        Speed of lerp zoom movement
float DistanceInfluence:                How much the distance influence the movement amount.
bool AutoRotate:                        Auto rotation enable?
CameraAutoRotationType AutoRotationType: The direction of the auto rotation types:
                                                                                  - Dynamically:  will modify the rotation direction from the last mouse / touch drag direction
																				  - Left:  Auto rotate to left
																				  - Right:  Auto rotate to right
float AutoRotSpeed:                     Speed of the Auto rotation
bool DetectCollision:                   Camera detect when an collider is between the camera and the target, then the camera will move to the next position where the collider is not in front
float CollisionRadius:                  The offset position from the hit collider (if there any)
bool FadeOnStart:                       Enable black fadeOut effect on start?

----------------------------------------------------------------------------------------------------------------

Change Target in RealTime:

If you want change the target to camera orbit in runtime, simple call the function "SetTarget(newTarget)" of bl_CameraOrbit.
where "newTarget" is the reference transform of new object that will orbit.

----------------------------------------------------------------------------------------------------------------

Remove the Zoom effect on start:

In the scene demo you will see a "zoom" effect on the start, this is a feature but if you don't want use it, simple need
set the variable "FogStart" of bl_CameraOrbit script to the same "fieldOfView" of CameraOrbit (60 for default).

----------------------------------------------------------------------------------------------------------------

Set the default position and rotation on start:

The orbit camera calculate the orbit movement depend of distance and rotation to be "circular" this mean that you can't see a default position
due this will be corrected since the first called of Update function (due the orbit operation), but we have made that take the most close position
to the default on start, but not will be exactly the same.

----------------------------------------------------------------------------------------------------------------

Remove the mini Zoom in Out when click and rotate:

For default (in the example scene and prefab) when you click to rotate the camera there are a small zoom effect, if you
want remove it simple set the variable in bl_CameraOrbit -> 'Puw Fog Amount' to = 0
this will remove the effect.

----------------------------------------------------------------------------------------------------------------

Contact / Support:
Forum: http://lovattostudio.com/forum/index.php
Email: brinerjhonson.lc@gmail.com