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

	Rigidbody RB;
	Transform TF;

	void OnEnable()
	{
		turretMovement = gameObject.GetComponentInChildren<gunController>();
		fireScript = gameObject.GetComponentInChildren<fireScript>();
		tankMovement = gameObject.GetComponent<TankMovement>();

		RB = this.GetComponent<Rigidbody>();
		TF = this.GetComponent<Transform>();
	}

	// Start is called before the first frame update
	void Start()
	{
        totalHealth = turretArmour.getHealth() + baseArmour.getHealth() + lTrackArmour.getHealth() + rTrackArmour.getHealth();
		initialHealth = totalHealth;
    }

	// Update is called once per frame
	void Update()
	{ 
		
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
		return totalHealth/initialHealth;
	}
}

