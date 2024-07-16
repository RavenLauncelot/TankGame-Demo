using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class fireScript : MonoBehaviour
{
	TankControls controls;
	InputAction fire;
	
	Rigidbody mainRB;
	
	Transform TF;
	Transform turretTF;
	
	ParticleSystem particleSys;
	List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
	public GameObject ricochetEffect;
	public GameObject hitEffect;
	
	bool canfire;
	public float reloadTime;
	public float recoilPower;
	//variables for different type of shells
	float penPower;
	float damage;
	
	void OnEnable()
	{
		controls = new TankControls();
		
		fire = controls.Tank.fire;
		fire.performed += shoot;
		
		controls.Enable();
		fire.Enable();
	}
	
	void OnDisable()
	{
		controls.Disable();
	}
	
	void Start()
	{
		canfire = true;
		particleSys = this.GetComponent<ParticleSystem>();
		ChangeShell("Standard");
		
		mainRB = this.GetComponentInParent<Rigidbody>();
		TF = this.GetComponent<Transform>();
		turretTF = GameObject.Find("gunPivot").GetComponent<Transform>();
		
	}
	
	void shoot(InputAction.CallbackContext obj)   //fire tank shell
	{
		if (canfire)
		{ 
			particleSys.Play(); 
			StartCoroutine((reloadTimer())); 
			
			Vector3 direction = turretTF.position - TF.position;  //the vector from this object (end of the barrel) to the gun pivot
			mainRB.AddForceAtPosition(direction.normalized * recoilPower, this.transform.position, ForceMode.Impulse);
			Debug.Log("Recoil direction: " + direction);
		}
	}
	
	private void OnParticleCollision(GameObject other)  //this is when the particle hits something. the particle system will be set to ignore the player so it wont hit itself when its fired
	{				
		if(other.TryGetComponent(out ArmourScript armour))
		{
			giveDamage(other, armour);
		}
	}
	
	private IEnumerator reloadTimer()
	{
		canfire = false;
		yield return new WaitForSeconds(reloadTime);
		canfire = true;
	}
	
	public void ChangeShell(string type)
	{
		if (type == "HE")
		{
			damage = 400;
			penPower = 200;
		}
		
		else if (type == "AP")
		{
			damage = 200;
			penPower = 400;
		}
		
		else if (type == "Standard")
		{
			damage = 300;
			penPower = 300;
		}
		
		else
		{
			//default to the standard shell
			Debug.Log("Invalid shell type - Defaulting to standard shell");
			
			damage = 300;
			penPower = 300;
		}
	}
	
	private void giveDamage(GameObject target, ArmourScript targetArmour)
	{
		//variables about the bullet itself
		float angleOfHit;
		float angleFactor;
		float penChance;   //the penetration chance considering the armour thickness

		//variables about the target its hitting
		float armourThickness;
		
		armourThickness = targetArmour.getThickness();
		int events = particleSys.GetCollisionEvents(target, colEvents); 
			
		for (int i = 0; i < events; i++)
		{
			//doing maths for the damage
			//in the calulations im using lots of things
			//these are the: angle of hit factor, penetration power, damage, armour thicknesss
				
			//first i will find the angle of the hit
			angleOfHit = Vector3.Angle(colEvents[i].velocity, -colEvents[i].normal);  //this is minus as it will give an obtuse angle if facing toward where the bullet is coming from and i want an angle between +- 0-90
				
			//for this to be made into a factor I need to convert it to a decimal 0-1
			angleFactor = (-1f/90f * angleOfHit) + 1f;  //This is basically just the opposite of multiplying angleofhit/90 * 1 which would give me the opposite of what i want. which is, lower the angle the higher the factor 0-1
			Debug.Log("Angle Factor: " + angleFactor + " Angle of hit: " + angleOfHit);
			
			//considering the armour thickness and the pentration values to find the final penetration chance
			penChance = (angleFactor * penPower) / armourThickness;    //this will find the total penetration power available and then divide it by the thickness to find the chance of it penetrating
																		   // this could result in some instances where it will always penetrate but that can be adjusted in game or in the editor 
																		   //if the chance of it always penetrating did happen it would just be a case of more powerufl gun or tank anyway
																		   
			if (Random.Range(0f, 1f) < penChance)  //if true it has penetrated the armour
			{
				//first need to get rid of the particle otherwise itd bounce off (like it didnt penetrate)
				Instantiate(hitEffect, colEvents[i].intersection, Quaternion.LookRotation(colEvents[i].normal));
				
				//the amount of damage it will give will depend on the armour thickness and its penetration
				//thicker armour more energy lost = less damage and vice versa
					
				//this could result in higher damage if the armour is weak
				float damageGiven = (penPower / armourThickness) * damage; 
					
				if (damageGiven > damage){ damageGiven = damage; }  //this counters having very high damage against weak armour so that shells dont go above their damage
					
				targetArmour.giveDamage(damageGiven);
				
				Debug.Log("Penetration, chance of hit: " + penChance);
			}
			
			else
			{
				Debug.Log("Ricochet, chance of hit: " + penChance);
				
				Instantiate(ricochetEffect, colEvents[i].intersection, Quaternion.LookRotation(Vector3.Reflect(-colEvents[i].velocity.normalized, colEvents[i].normal)));
			}
		}
	}
}
