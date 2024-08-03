using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
	//this code was abandoned due to time limitations
	//code worked but didn't aim correctly 
	
	[SerializeField] private Transform pivot;

	[SerializeField] private float range;
	[SerializeField] private float turretSpeed;
	[SerializeField] private float penPower;
	[SerializeField] private float damage;
	
	Transform TF;
	[SerializeField] private Transform player;

	private ParticleSystem particleSys;
	[SerializeField] private GameObject hitEffect;
	[SerializeField] private GameObject ricochetEffect;
	
	private ArmourScript turretBody;
	private ArmourScript turretGun;
	
	void Start()
	{
		TF = this.GetComponent<Transform>();
		particleSys = this.GetComponentInChildren<ParticleSystem>();
		
		ArmourScript[] tempArmours = this.GetComponentsInChildren<ArmourScript>();
		
		foreach(ArmourScript script in tempArmours)   //finding the armourscript in this object and assinging it to the appropiate variable 
		{
			if (script.getName() == "body")
			{
				turretBody = script;
			}
			else if (script.getName() == "gun")
			{
				turretGun = script;
			}
		}
		
		StartCoroutine(aimAtPlayer());
	}

	public void Update()
	{
		
	}

	IEnumerator aimAtPlayer()
	{
		while (true)
		{
			//Debug.Log("Current distance: " + Vector3.Distance(TF.position, player.position));
			Vector3 turretToPlayer;
			
			while (Vector3.Distance(TF.position, player.position) < range)   //aim at player stop once is aimed at player and aim again after set time
			{
				turretToPlayer = player.position - pivot.position;
				pivot.rotation = Quaternion.RotateTowards(pivot.rotation, Quaternion.LookRotation(turretToPlayer, Vector3.up), turretSpeed*Time.deltaTime);				
					
				RaycastHit hit;
				Vector3 rayPosition = pivot.position + pivot.forward * 5f;  //this will position the ray out the front of thebarrel so it doesnt collide with itself
				Ray ray = new Ray(rayPosition, pivot.forward);
				
				if (Physics.Raycast(ray, out hit, range))
				{
					Debug.Log("Raycast tag: " + hit.collider.tag);
					Debug.DrawRay(rayPosition, pivot.forward * range, Color.red);
					if (hit.collider.tag == "Player")
					{
						Debug.Log("Firing");
						Fire();
						yield return new WaitForSeconds(5);    //once its fired at the player it will stay in the same place till the time is up
					}
				}
				yield return null;	
			}
			yield return new WaitForEndOfFrame();
		}
	}
	
	private void Fire()
	{
		//Debug.Log("Enemy turret fired");
		particleSys.Play();  //fires a particle using particlesystem
	}

	private void OnParticleCollision(GameObject other)  //this is when the particle hits something. the particle system will be set to ignore the player so it wont hit itself when its fired
	{
		if (other.GetComponentInChildren<ArmourScript>() != null) 
		{
			giveDamage(other);
		}
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
				targetArmour = armour;
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

				Debug.Log("Penetration, chance of hit: " + penChance);
			}

			else
			{
				Debug.Log("Ricochet, chance of hit: " + penChance);

				Instantiate(ricochetEffect, colEvents[i].intersection, Quaternion.LookRotation(Vector3.Reflect(colEvents[i].velocity.normalized, colEvents[i].normal)));
			}
		}
	}
}
