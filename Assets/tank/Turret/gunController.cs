using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.TextCore.LowLevel;

public class gunController : MonoBehaviour
{

    [SerializeField] private float turretYawSpeed;
    [SerializeField] private float turretPitchSpeed;

    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;


    [SerializeField] private float currentGunPitch;

    [SerializeField] private float currentCamPitch;

	float previousCameraPitch;

	public Transform camPivotY;
	public Transform camPivotX;

	public Transform gunPivotX;
	Transform gunPivotY;
	
	public cameraController camController;

	private void Start()
	{
		gunPivotY = this.GetComponent<Transform>();

		currentGunPitch = 0;
		currentCamPitch = 0;
		gunPivotX.localEulerAngles = Vector3.zero;
	}

	private void FixedUpdate()  //well be doing it in fixed update since this object is connected to a rigidbody
	{
		currentCamPitch = camController.getCamXAngle();
		
		Quaternion turretAngleY = getTargetDirection("y");
		Quaternion turretAngleX = getTargetDirection("x");

		//this for the spinning there are no limits so this is very straight forward
		gunPivotY.localRotation = Quaternion.RotateTowards(gunPivotY.localRotation, turretAngleY, turretYawSpeed * Time.deltaTime);   //because ive split the axis into seperate gameobjects in the editor i dont need need to single out the axis i want to move 

		//because thee x axis has to move within limit I had to change the code to clamp it if it moves out of bounds
		Vector3 pitchAmount = new Vector3(pitchDirection(gunPivotX.localRotation, turretAngleX)*turretPitchSpeed*Time.deltaTime, 0, 0);
		pitchAmount.x = Mathf.Clamp(pitchAmount.x, minPitch - currentGunPitch, maxPitch - currentGunPitch);
		
		float difference = Mathf.Clamp(currentCamPitch, minPitch, maxPitch) - currentGunPitch;
		
		if (Mathf.Abs(pitchAmount.x) > Mathf.Abs(difference))   //if the pitchamount will overshoot the angle the camera is facing it will directly set it to its rotaion by the amount required
		{
			pitchAmount.x = difference;  //i need to set this to pitchamoutn otherwise it will lose track of its position when its added to currentGunPitch it also means less if statements 
		}
		else
		{
			difference = 999;
		}

		gunPivotX.Rotate(pitchAmount);
		
		currentGunPitch += pitchAmount.x;

		Debug.DrawRay(gunPivotX.position, gunPivotX.forward * 100, Color.red);
		Debug.DrawRay(gunPivotY.position, gunPivotY.forward * 100, Color.green);
	}
	
	public Quaternion getTargetDirection(string axis)    //this finds the global rotation of the vector from the tank gun to the postion the camera is looking at.
	{
		Vector3 direction = new Vector3(0,0,0);
		Quaternion rotationDifference;
		
		Vector3 targetPos = camController.getTarget();
		
		if (axis == "y")   //these are the axis specifcally for the gun 
		{
			direction.x = targetPos.x - gunPivotY.position.x;   //getting the relevant positions coordinates for the axis 
			direction.z = targetPos.z - gunPivotY.position.z;
			direction.y = 0;

			//Debug.Log("Y axis target vector: " + direction);
			rotationDifference = Quaternion.LookRotation(direction, Vector3.up);   //this finds the rotation of the vector from the gun to the target the cameras looking at
			return rotationDifference;
		}
		else if (axis == "x")
		{		
			//since this is angle of the X axis i cant just get the y positon otherwise it would just point vertical
			//I also need it so that the direction is straight ahead of the gun so it rotates on the correct axis this mean i need to find the lenght of the hypotenuse of the x and z values and then point it forward ahead of the gun to get the correct rotation
			float distanceToTarget = Mathf.Sqrt(Mathf.Pow(targetPos.x - gunPivotX.position.x, 2) + Mathf.Pow(targetPos.z - gunPivotX.position.z, 2));       //using a^2 + b^2 = c^2  - once ive found the distance to the target im going to point it in the same direction as the gun
			direction.z = distanceToTarget;
			direction.y = targetPos.y - gunPivotX.position.y;
			direction.x = 0;

			//Debug.Log("X axis target vector: " + direction);

			rotationDifference = Quaternion.LookRotation(direction ,Vector3.up);
			return rotationDifference;
		}
		else
		{
			return Quaternion.Euler(0,0,0); //error
		}
	}
	
	private float pitchDirection(Quaternion from, Quaternion to)   //this is basically a factor for the code above mulitplying depending on what direction it needs to go
	{
		float fromX = from.eulerAngles.x;
		float toX = to.eulerAngles.x;
		
		float up;
		float down;
		
		if (fromX <= toX)
		{
			up = toX - fromX;
			down = fromX + (360f - toX);
		}
		
		else
		{
			up = toX + (360f - fromX);
			down = fromX - toX;
		}
		
		if (up < down)
		{
			return 1f;
		}
		else
		{
			return -1f;
		}
	}

	public void adjustTurretYaw(float modifier)
	{
		if (modifier < 0f || modifier > 1f)  //this checks if the input is invalid or not 
		{
			return;  //if its invalid it breaks
		}

		turretYawSpeed = turretYawSpeed * modifier;
	}

	public void adjustTurretPitch(float modifier)
	{
        if (modifier < 0f || modifier > 1f)  //this checks if the input is invalid or not 
        {
            return;  //if its invalid it breaks
        }

		turretPitchSpeed = turretPitchSpeed * modifier;
    }
}
