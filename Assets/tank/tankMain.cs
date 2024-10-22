using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class tankMain : MonoBehaviour
{

	private TankControls controls;  //geting the script generated from the unity input system

	//I need these to adjust the values for when a part gets damaged
	private gunController turretMovement;
	private fireScript fireScript;
	private TankMovement tankMovement;

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

	void Awake()
	{
		controls = new TankControls();
	}

	void OnEnable()
	{
		turretMovement = gameObject.GetComponentInChildren<gunController>();
		fireScript = gameObject.GetComponentInChildren<fireScript>();
		tankMovement = gameObject.GetComponent<TankMovement>();

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
		initialHealth = totalHealth;

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
		tankMovement.setTorqueModifier(modifier);
		
		
		
		//track modifiers
		modifier = Mathf.Clamp(lTrackArmour.getHealthPercent(), 0.5f, 1f) + Mathf.Clamp(rTrackArmour.getHealthPercent(), 0.5f, 1f);
		modifier /= 2f;   //finds the average health between the 2
		modifier = modifier - Mathf.Clamp(Random.Range(0.0f, 0.5f) * (1f - turretArmour.getHealthPercent()), 0f, 1f);
		tankMovement.setSpeedModifier(modifier);
		
		if (totalHealth < 0)
		{
			//health too low
			Destroy(this.gameObject);
		}	
	}	

	public float getHealthPercentage()
	{
		return initialHealth/totalHealth;
	}
}

