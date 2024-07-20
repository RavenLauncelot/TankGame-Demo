using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class followCameraAngle : MonoBehaviour
{

	public float turretYawSpeed;
	public float turretPitchSpeed;

	public float minPitch;
	public float maxPitch;

	
	public float currentGunPitch;
	
	public float currentCamPitch;
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
		
		//this for the spinning there are no limits so this is very straight forward
		gunPivotY.localRotation = Quaternion.RotateTowards(gunPivotY.localRotation, camPivotY.localRotation, turretYawSpeed * Time.deltaTime);   //because ive split the axis into seperate gameobjects in the editor i dont need need to single out the axis i want to move 

		//because thee x axis has to move within limit I had to change the code to clamp it if it moves out of bounds
		Vector3 pitchAmount = new Vector3(pitchDirection(gunPivotX.localRotation, camPivotX.localRotation)*turretPitchSpeed*Time.deltaTime, 0, 0);
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
}
