using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using Unity.Properties;
using UnityEngine.ParticleSystemJobs;
using JetBrains.Annotations;

public class tankMain : MonoBehaviour
{

	public TankControls controls;  //geting the script generated from the unity input system

	//setting up the individual control inputs
	private InputAction movement;
	private InputAction turret;
	private InputAction fire;

	//these will hold the values of the inputs during the update method alot shorter than left.readvalue<float>() so makes it a bit more readable and it will only need to get the input once everyframe rather than whenever its used.
	private Vector2 movementVecIn;
	private Vector2 turretVecIn;

	//getting all the necesarray objects and components that make up the tank so that i can access all their properties
	public GameObject TankTrackL ,TankTrackR;
	public Transform TurretPivot;
	public Transform GunPivot;
	
	private ArmourScript[] armourAreas;

	private WheelCollider[] leftTrack;
	private WheelCollider[] rightTrack;

	Rigidbody RB;
	Transform TF;

	//set variables for things like speed
	public float maxTurretSpeed = 10f;
	public float maxGunSpeed = 10f;
	
	public float maxTurnSpeed = 300;    
	public float maxSpeed = 500;
	public float initialTorque = 2000;
	public float brakingTorque = 1000;
	
	private float maxGunPitch = 11.89f;
	private float minGunPitch = -25f;
	private float currentGunPitch = 0;

	//debug public variables
	//public float currentTurretRotation;
	private float currentSpeed;

	//public float rTrackTorque;  //old debug variables im keeping them commented just in case i need them
	//public float lTrackTorque;
	//public float rBrakingTorque;
	//public float lBrakingTorque;
	//public float rWheelSpeed;
	//public float lWheelSpeed;

	void Awake(){
		controls = new TankControls();

		//TankTrackL ,TankTrackR.GetComponent<Rigidbody>().
	}

	void OnEnable(){
		//enabling each control for the tank 
		movement = controls.Tank.movement;
		turret = controls.Tank.turret;

		movement.Enable();
		turret.Enable();

		RB = this.GetComponent<Rigidbody>();
		TF = this.GetComponent<Transform>();
	}

	void OnDisable(){
		controls.Disable();
	}


	// Start is called before the first frame update
	void Start()
	{
		//setting up the wheel colliders 
		leftTrack = TankTrackL.GetComponentsInChildren<WheelCollider>();
		rightTrack = TankTrackR.GetComponentsInChildren<WheelCollider>();
		
		GunPivot.localEulerAngles = Vector3.zero;
		
		armourAreas = this.GetComponentsInChildren<ArmourScript>();
	}

	// Update is called once per frame
	void Update(){ //this is mainly for debug variables and updating the input 
		//updating debug variables
		//currentTurretRotation = TurretPivot.localEulerAngles.y;
		//currentSpeed = Vector3.Dot(transform.forward, RB.velocity);

		//rTrackTorque = rightTrack[0].motorTorque;                //these debug values arent used anymore
		//lTrackTorque = leftTrack[0].motorTorque;
		//rBrakingTorque = rightTrack[0].brakeTorque;
		//lBrakingTorque = leftTrack[0].brakeTorque;
		//rWheelSpeed = rightTrack[0].rotationSpeed;
		//lWheelSpeed = leftTrack[0].rotationSpeed;

		//getting inputs
		turretVecIn = turret.ReadValue<Vector2>();
		movementVecIn = movement.ReadValue<Vector2>();
	}

	void FixedUpdate(){
		//doing movement for the turret 

		//Vector3 turretYaw = new Vector3(0 ,0 ,0);
		//turretYaw.y = turretVecIn.x * maxTurretSpeed * Time.deltaTime;
		//TurretPivot.Rotate(turretYaw);

		//Vector3 turretPitch = new Vector3(0 , 0 ,0);
		//turretPitch.x = turretVecIn.y * maxTurretSpeed * Time.deltaTime;
		//turretPitch.x = Mathf.Clamp(turretPitch.x, minGunPitch-currentGunPitch, maxGunPitch-currentGunPitch);
		//GunPivot.Rotate(turretPitch);
		
		//currentGunPitch += turretPitch.x;

		trackMovement(rightTrack, leftTrack, movementVecIn.x, movementVecIn.y);  //this is all dealt within a seperate function. Doing this won't make it more effcient but it will make the code a lot more readable as whole.
	}


	private void trackMovement(WheelCollider[] rTrack, WheelCollider[] lTrack, float inputX, float inputY)
	{
		float wheelSpeed;
		if (inputX == 0 && inputY == 0)
		{
			foreach (WheelCollider sprocket in rTrack)
			{
				sprocket.brakeTorque = 200;
				sprocket.motorTorque = 0;          
			}

			foreach (WheelCollider sprocket in lTrack)
			{
				sprocket.brakeTorque = 200;
				sprocket.motorTorque = 0;
			}

			return;
		}

		//there's going to be 2 modes for the tank very similar to a real life tank
		//one is going to be called neutral steering and the other is going to be pivot steering
		//if the tank is not moving it will and the stick is pointed far right or left it will neutral steer
		//if its pointed mostly forward or backward it will pivot steer

		//if statement for neutral steering mode (both track moving in opposite directions)
		if (movementVecIn.x < -0.8f | movementVecIn.x > 0.8f)  //if the stick is pointed far right or left
		{
			//if the user wants to go right it will give a positive value so ill need to invert it as the right track will need to go back. 
			float invIn = -inputX;

			foreach (WheelCollider sprocket in rTrack)
			{
				wheelSpeed = sprocket.rotationSpeed;

				sprocket.brakeTorque = 0;
				sprocket.motorTorque = Mathf.Lerp(invIn * initialTorque, 0, Mathf.Abs(wheelSpeed)/maxTurnSpeed);          
			}

			foreach (WheelCollider sprocket in lTrack)
			{
				wheelSpeed = sprocket.rotationSpeed;

				sprocket.brakeTorque = 0;
				sprocket.motorTorque = Mathf.Lerp(inputX * initialTorque, 0, Mathf.Abs(wheelSpeed)/maxTurnSpeed);
			}

			//Debug.Log("Neutral steering");       
		} 

		//we need to specify a zone while trying to go straight otherwise it will be very difficult to keep it going straight
		else if (movementVecIn.x > -0.2 && movementVecIn.x < 0.2)
		{
			foreach (WheelCollider sprocket in rTrack)
			{
				wheelSpeed = sprocket.rotationSpeed;

				//checks whether its accelerating or decelerating/braking
				if (wheelSpeed * inputY >= 0) //originally i multiplied them both together and checked if they were above 0 but i believe this may be more effcient way to check if they share the same sign
				{
					sprocket.brakeTorque = 0;
					sprocket.motorTorque = Mathf.Lerp(inputY * initialTorque, 0, Mathf.Abs(wheelSpeed)/maxSpeed);
				}

				else //decelerating / braking
				{   
					sprocket.motorTorque = 0;
					sprocket.brakeTorque = brakingTorque * Mathf.Abs(inputY);
				}
			}

			foreach (WheelCollider sprocket in lTrack)
			{
				wheelSpeed = sprocket.rotationSpeed;

				//checks whether its accelerating or decelerating/braking
				if (wheelSpeed * inputY >= 0) //originally i multiplied them both together and checked if they were above 0 but i believe this may be more effcient way to check if they share the same sign
				{
					sprocket.brakeTorque = 0;
					sprocket.motorTorque = Mathf.Lerp(inputY * initialTorque, 0, Mathf.Abs(wheelSpeed)/maxSpeed);
				}

				else
				{   
					sprocket.motorTorque = 0;
					sprocket.brakeTorque = brakingTorque * Mathf.Abs(inputY);
				}
				
			}

			//Debug.Log("Going straight");
		}

		//this will be pivot steering one track will be locked while the other will drive the tank
		else
		{

			float combinedInput = Mathf.Abs(inputX + inputY);
			combinedInput /= 2;

			//need to figure out if its going left or right
			if (movementVecIn.x > 0) //true if its turning right 
			{
				foreach (WheelCollider sprocket in rTrack)
				{
					sprocket.motorTorque = 0;
					sprocket.brakeTorque = brakingTorque * Mathf.Abs(inputX * 0.5f);
				}
				foreach (WheelCollider sprocket in lTrack)
				{
					wheelSpeed = sprocket.rotationSpeed;

					sprocket.brakeTorque = 0;
					sprocket.motorTorque = Mathf.Lerp(initialTorque * combinedInput, 0, Mathf.Abs(wheelSpeed)/maxSpeed);
				}
			}

			else       //true if its turning left 
			{
				foreach (WheelCollider sprocket in rTrack)
				{
					wheelSpeed = sprocket.rotationSpeed;

					sprocket.brakeTorque = 0;
					sprocket.motorTorque = Mathf.Lerp(initialTorque * combinedInput, 0, Mathf.Abs(wheelSpeed)/maxSpeed);
				}
				foreach (WheelCollider sprocket in lTrack)
				{
					sprocket.motorTorque = 0;
					sprocket.brakeTorque = brakingTorque * Mathf.Abs(inputX * 0.5f);
				}
			}
		}      
	}

	private ArmourScript getArmourByName(string name)
	{
		foreach (ArmourScript a in armourAreas)
		{
			if (a.getName() == name)
			{
				return a;
			}
		}
		
		return null;
	}

	//private Vector2 InputSmoothing(Vector2 input)
	//{
	//	get the input
	//	return null;
	//}
}

 
