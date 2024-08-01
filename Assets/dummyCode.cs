using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummyCode : MonoBehaviour
{
	[SerializeField] private ArmourScript tankBase;
	[SerializeField] private ArmourScript tankTurret;
	[SerializeField] private ArmourScript trackL;
	[SerializeField] private ArmourScript trackR;
	
	private float initialHealth;
	private float health;
	
	// Start is called before the first frame update
	void Start()
	{
		initialHealth = tankBase.getHealth() + tankTurret.getHealth() + trackL.getHealth() + trackR.getHealth();
		health = initialHealth;
	}

	// Update is called once per frame
	void Update()
	{
		if (tankBase.getHealth() == 0)   //base of tank is destroyed 
		{
			GameObject.Destroy(this);
		}
		
		else if (health/initialHealth < 0.1)  //if overall health is less than 10% tank gets destroyed
		{
			GameObject.Destroy(this);
		}
	}
}
