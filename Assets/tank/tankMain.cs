using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Properties;
using UnityEngine.ParticleSystemJobs;
using JetBrains.Annotations;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

public class tankMain : MonoBehaviour
{

	public TankControls controls;  //geting the script generated from the unity input system

	//I need these to adjust the values for when a part gets damaged
	public gunController turretMovement;
	public fireScript fireScript;

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

	//variables for things like speed
    [SerializeField] private float maxTurnSpeed = 300;
    [SerializeField] private float maxSpeed = 500;
    [SerializeField] private float initialTorque = 2000;
    [SerializeField] private float brakingTorque = 1000;

	//these modifie the amount of torque a track receives
	private float rTrackModifier = 1f;
	private float lTrackModifier = 1f;

	private float currentSpeed;

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
		
		GunPivot.localEulerAngles = Vector3.zero;

		part turretArmour = new part(getArmourByName("turret"));
		turretArmour.minMaxJitter(0.1f, 0.2f);
		turretArmour.minModifier(0.5f);
	}

	// Update is called once per frame
	void Update()
	{ 
		//getting inputs
		turretVecIn = turret.ReadValue<Vector2>();
		movementVecIn = movement.ReadValue<Vector2>();
	}

	void FixedUpdate()
	{
		trackMovement(rightTrack, leftTrack, movementVecIn.x, movementVecIn.y);  //this is all dealt within a seperate function. Doing this won't make it more effcient but it will make the code a lot more readable as whole.
	}


	private void trackMovement(WheelCollider[] rTrack, WheelCollider[] lTrack, float inputX, float inputY)
	{
		float wheelSpeed;
		if (inputX == 0 && inputY == 0)
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
}

public class part
{
	ArmourScript armour;
	private float minimumMod;
	private float maxJitter;
	private float minJitter;
	private float initialHealth;

	public part(ArmourScript ImpArmour)
	{
		armour = ImpArmour;
		initialHealth = armour.getHealth();
	}
	
	public void getModifier()
	{
		float modifier = armour.getHealth() / initialHealth;
		modifier =+ Random.Range(minJitter, maxJitter);
	}

	public void minModifier(float minimum)
	{
		if (minimum > 1)
		{
			return;
		}

		minimumMod = minimum;
	}

	public void minMaxJitter(float minimum, float maximum)
	{
		minJitter = minimum;
		maxJitter = maximum;
	}

	public float getHealthPercentage()
	{
		return armour.getHealth() / initialHealth;
	}
}


