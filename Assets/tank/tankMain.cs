using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class tankMain : MonoBehaviour
{

	[SerializeField] private TankControls controls;  //geting the script generated from the unity input system

	//I need these to adjust the values for when a part gets damaged
	[SerializeField] private gunController turretMovement;
	[SerializeField] private fireScript fireScript;

	//setting up the individual control inputs
	private InputAction movement;
	private InputAction turret;

	//these will hold the values of the inputs during the update method alot shorter than left.readvalue<float>() so makes it a bit more readable and it will only need to get the input once everyframe rather than whenever its used.
	private Vector2 movementVecIn;

	//getting all the necesarray objects and components that make up the tank so that i can access all their properties
	[SerializeField] private GameObject TankTrackL ,TankTrackR;
	[SerializeField] private Transform TurretPivot;
	[SerializeField] private Transform GunPivot;
	
	[SerializeField] private ArmourScript turretArmour;
	[SerializeField] private ArmourScript lTrackArmour;
	[SerializeField] private ArmourScript rTrackArmour;
	[SerializeField] private ArmourScript baseArmour;
	
	private float totalHealth;
	private float initialHealth;

	private WheelCollider[] leftTrack;
	private WheelCollider[] rightTrack;

	Rigidbody RB;
	Transform TF;

	//variables for things like speed
	[SerializeField] private float turnSpeed;
	[SerializeField] private float speed;
	[SerializeField] private float torque;
	[SerializeField] private float brakingTorque;
	
	[SerializeField] private float maxTurnSpeed = 250;
	[SerializeField] private float maxSpeed = 1000;
	[SerializeField] private float maxTorque = 2000;
	[SerializeField] private float maxBrakingTorque = 2000;
	
	//UI 
	[SerializeField]private Text healthUI;

	void Awake()
	{
		controls = new TankControls();
	}

	void OnEnable()
	{
		//enabling each control for the tank 
		movement = controls.Tank.movement;
		turret = controls.Tank.turret;

		movement.Enable();
		turret.Enable();

		RB = this.GetComponent<Rigidbody>();
		TF = this.GetComponent<Transform>();
	}

	void OnDisable()
	{
		controls.Disable();
	}


	// Start is called before the first frame update
	void Start()
	{		
		//setting up the wheel colliders 
		leftTrack = TankTrackL.GetComponentsInChildren<WheelCollider>();
		rightTrack = TankTrackR.GetComponentsInChildren<WheelCollider>();	
		
		totalHealth = turretArmour.getHealth() + baseArmour.getHealth() + lTrackArmour.getHealth() + rTrackArmour.getHealth();

		brakingTorque = maxBrakingTorque;
		turnSpeed = maxTurnSpeed;
		torque = maxTorque;
		speed = maxSpeed;
	}
	


	// Update is called once per frame
	void Update()
	{ 
		movementVecIn = movement.ReadValue<Vector2>();
		
		//updating damage things 
		totalHealth = turretArmour.getHealth() + baseArmour.getHealth() + lTrackArmour.getHealth() + rTrackArmour.getHealth();
		
		//updating the modifiers
		//turret modifiers
		float modifier = Mathf.Clamp(turretArmour.getHealthPercent(), 0.5f, 1f);		
		modifier = modifier - Mathf.Clamp(Random.Range(0.0f, 0.5f) * (1f - turretArmour.getHealthPercent()),0 ,1);	
		turretMovement.modTurretPitch(modifier);
		turretMovement.modTurretYaw(modifier);
		
		//base modifiers
		modifier = Mathf.Clamp(baseArmour.getHealthPercent(), 0.5f, 1f);
		modifier = modifier - Mathf.Clamp(Random.Range(0.0f, 0.5f) * (1f - baseArmour.getHealthPercent()),0, 1);
		setTorqueModifier(modifier);
		setTurningModifier(modifier);
		
		
		//track modifiers
		modifier = Mathf.Clamp(lTrackArmour.getHealthPercent(), 0.5f, 1f) + Mathf.Clamp(rTrackArmour.getHealthPercent(), 0.5f, 1f);
		modifier /= 2f;   //finds the average health between the 2
		modifier = modifier - Mathf.Clamp(Random.Range(0.0f, 0.5f) * (1f - turretArmour.getHealthPercent()), 0f, 1f);
		setSpeedModifier(modifier);
		
		if (totalHealth < initialHealth/4f)
		{
			//health too low
			Destroy(this.gameObject);
		}
		
		healthUI.text = "Healh: " + totalHealth;  //setting the health
	}	

	void FixedUpdate()
	{
		trackMovement(rightTrack, leftTrack, movementVecIn.x, movementVecIn.y);  //this is all dealt within a seperate function. Doing this won't make it more effcient but it will make the code a lot more readable as whole.
	}


	private void trackMovement(WheelCollider[] rTrack, WheelCollider[] lTrack, float inputX, float inputY)
	{
		float wheelSpeed;
		if (inputX == 0 && inputY == 0)   //when input is zero it will slowly come to a stop and hold its brakes
		{
			foreach (WheelCollider sprocket in rTrack)
			{
				sprocket.brakeTorque = 400;
				sprocket.motorTorque = 0;          
			}

			foreach (WheelCollider sprocket in lTrack)
			{
				sprocket.brakeTorque = 400;
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
				sprocket.motorTorque = Mathf.Lerp(invIn * torque, 0, Mathf.Abs(wheelSpeed)/turnSpeed);          
			}

			foreach (WheelCollider sprocket in lTrack)
			{
				wheelSpeed = sprocket.rotationSpeed;

				sprocket.brakeTorque = 0;
				sprocket.motorTorque = Mathf.Lerp(inputX * torque, 0, Mathf.Abs(wheelSpeed)/turnSpeed);
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
					sprocket.motorTorque = Mathf.Lerp(inputY * torque, 0, Mathf.Abs(wheelSpeed)/speed);
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
					sprocket.motorTorque = Mathf.Lerp(inputY * torque, 0, Mathf.Abs(wheelSpeed)/speed);
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
					sprocket.motorTorque = Mathf.Lerp(torque * combinedInput, 0, Mathf.Abs(wheelSpeed)/speed);
				}
			}

			else       //true if its turning left 
			{
				foreach (WheelCollider sprocket in rTrack)
				{
					wheelSpeed = sprocket.rotationSpeed;

					sprocket.brakeTorque = 0;
					sprocket.motorTorque = Mathf.Lerp(torque * combinedInput, 0, Mathf.Abs(wheelSpeed)/speed);
				}
				foreach (WheelCollider sprocket in lTrack)
				{
					sprocket.motorTorque = 0;
					sprocket.brakeTorque = brakingTorque * Mathf.Abs(inputX * 0.5f);
				}
			}
		}      
	}

	public void setSpeedModifier(float mod)
	{	
		speed = maxSpeed * mod;
	}

	public void setTurningModifier(float mod)
	{
		turnSpeed = maxTurnSpeed * mod;
	}
	
	public void setTorqueModifier(float mod)
	{
		torque = maxTorque * mod;
	}
}

