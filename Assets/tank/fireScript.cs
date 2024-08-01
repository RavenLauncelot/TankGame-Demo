using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class fireScript : MonoBehaviour
{
	TankControls controls;
	InputAction fire;
	InputAction changeShell;
	
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
	
	int shellType = 0;
	
	
	[SerializeField] private Text shotReportUI;
	[SerializeField] private Text shellTypeUI;
	
	void OnEnable()
	{
		controls = new TankControls();
		
		fire = controls.Tank.fire;
		fire.performed += shoot;
		
		changeShell = controls.Tank.changeShell;
		changeShell.performed += ChangeShell;
		
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
			mainRB.AddForceAtPosition(direction.normalized * recoilPower, this.transform.position, ForceMode.Impulse);   //this simulates recoil and pushed back on the tankgun
		}
	}
	
	private void OnParticleCollision(GameObject other)  //this is when the particle hits something. the particle system will be set to ignore the player so it wont hit itself when its fired
	{				
		if(other.GetComponentInChildren<ArmourScript>() != null)
		{
			giveDamage(other);
		}
	}
	
	private IEnumerator reloadTimer()
	{
		canfire = false;
		yield return new WaitForSeconds(reloadTime);
		canfire = true;
	}
	
	private void ChangeShell(InputAction.CallbackContext obj)
	{
		string type = "";
		shellType++;
		if (shellType > 2){ shellType = 0; }
		
		switch(shellType)
		{
		case 2:
			damage = 400;
			penPower = 200;
			type = "HE";
			break;
			
		case 1:
			damage = 200;
			penPower = 400;
			type = "AP";
			break;
			
		case 0:
			damage = 300;
			penPower = 300;
			type = "Standard";
			break;
			
		default:
			
			break;
		}
		
		shellTypeUI.text = "Shell Type: " + type;
		
	}

	private void giveDamage(GameObject target)
	{
		//variables about the bullet itself
		float angleOfHit;
		float angleFactor;
		float penChance;   //the penetration chance considering the armour thickness

		//list for the collision events
		List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();

		//variables about the target its hitting
		ArmourScript targetArmour;

		int events = particleSys.GetCollisionEvents(target, colEvents);

		for (int i = 0; i < events; i++)
		{
			if (colEvents[i].colliderComponent.TryGetComponent<ArmourScript>(out ArmourScript armour))
			{
				targetArmour = armour;   //this means it hit a gameobject child or not and it had a armourscript component
			}
			else
			{
				return;  //did not hit a gameobject with a armourscript component 
			}

			float armourThickness = targetArmour.getThickness();

			//doing maths for the damage
			//in the calulations im using lots of things
			//these are the: angle of hit factor, penetration power, damage, armour thicknesss

			//first i will find the angle of the hit
			angleOfHit = Vector3.Angle(colEvents[i].velocity, -colEvents[i].normal);  //this is minus as it will give an obtuse angle if facing toward where the bullet is coming from and i want an angle between +- 0-90

			//for this to be made into a factor I need to convert it to a decimal 0-1
			angleFactor = (-1f / 90f * angleOfHit) + 1f;  //This is basically just the opposite of multiplying angleofhit/90 * 1 which would give me the opposite of what i want. which is, lower the angle the higher the factor 0-1

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

				if (damageGiven > damage) { damageGiven = damage; }  //this counters having very high damage against weak armour so that shells dont go above their damage

				targetArmour.giveDamage(damageGiven);
				shotReportUI.text = "Last shot: hit!";
			}

			else
			{
				Instantiate(ricochetEffect, colEvents[i].intersection, Quaternion.LookRotation(Vector3.Reflect(colEvents[i].velocity.normalized, colEvents[i].normal)));
				shotReportUI.text = "Last shot: Ricochet!";
			}
		}
	}

	public Vector3 getTarget()   
	{
		Debug.DrawRay(TF.position, TF.forward * 100, Color.blue);

		Ray ray = new Ray(TF.position, TF.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			Debug.Log("Target: " + hit.point);
			return hit.point;	
		}

		else
		{
			Debug.Log("Not Hitting");
			return ray.GetPoint(2000);
		}
	}
}
