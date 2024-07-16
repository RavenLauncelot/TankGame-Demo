using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class shellScript : MonoBehaviour
{
	public float damage;
	public float penetrationFactor;

	public bool fired;
	private Rigidbody rb;
	private Transform tf;
	private MeshCollider mc;

	public Transform restPosition;
	
	Vector3 heading;
	
	//these are frequenly used variables so im defining them here to save time
	RaycastHit hit;
	float angleOfHit;
	Vector3 currentVelocityVec;
	bool hitObject;

	void Start()
	{
		rb = this.GetComponent<Rigidbody>();
		tf = this.GetComponent<Transform>();
		mc = this.GetComponentInChildren<MeshCollider>();
		fired = false;		
		hitObject = false;
	}

	void Update()
	{
		
		//this is for specifically orientating the shell while in and not in movement
		if (fired == false)
		{
			goToRest();
		}

		else if (fired == true)
		{
			heading = tf.position + currentVelocityVec;
			tf.LookAt(heading);  //this will orientate the shell towards where its going.
		}
	}
	
	void FixedUpdate()
	{
		Debug.DrawRay(transform.position, transform.forward, Color.red, 0.5f);
		
		currentVelocityVec = rb.velocity.normalized;
		
		if (!hitObject && Physics.Raycast(transform.position, transform.forward, out hit, 2f))
		{
			if (hit.collider.tag == "Enemy" | hit.collider.tag == "Armour")
			{
				//doing the maths to see whether it penetrates not 
				
				angleOfHit = Vector3.Angle(currentVelocityVec, -hit.normal);   //since the hit.normal goes toward where the bullet is firing it was giving me angles for an obtuse triangle in the range of 90 - 180  and -90 - -180
																			   //ive inverted the hit.normal so the vector i get from the face goes in the same direction as the bullet giving me a smaller range of 0-90 which works better for my needs
				Debug.Log("Hit plane normal: " + hit.normal);
				Debug.Log("Hit at angle: " + angleOfHit);
				hitObject = true;
			}
		}
	}
	
	void OnCollisionEnter()
	{
		
	}

	public void shellFired(float shellVelocity)
	{
		//mc.enabled = true;     //starts by enabling the collider so it can collide with objects
		fired = true;          //this is so it starts directing itself in the direction of travel
		rb.isKinematic = false;
		rb.velocity = tf.forward * shellVelocity;            //firing the shell this only works if isKinematic is set to false
		StartCoroutine(firedTimer());  //timer for the shell till it return to the tank
	}

	public IEnumerator firedTimer()
	{
		yield return new WaitForSeconds(2);
		fired = false;
		rb.velocity = Vector3.zero;
		goToRest();
	}
	
	void goToRest()
	{
		mc.enabled = false;
		this.transform.position = restPosition.position;
		this.transform.localRotation = restPosition.rotation;    //the rest position is pointed in the direction the of the shell so no offset is needed
		rb.isKinematic = true;
		hitObject = false;
	}

}

