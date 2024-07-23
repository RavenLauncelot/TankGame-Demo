using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourScript : MonoBehaviour
{
	//This script is going to be used on multiple differnet types of objects 
	//I did this so that i dont need to program my particlesystem to work with lots of different scripts
	
	public float setHealth;         //this is so i can set the health in the editor then the private version are set to these values
	public float setThickness;
	public string setPartName;
	
	public float health;
	private float armourThickness;
	private string partName;
	
	Renderer rend;
	
	private void Start()
	{
		rend = this.GetComponent<Renderer>();
		health = setHealth;
		armourThickness = setThickness;
		partName = setPartName;
	}
	
	private void Update()
	{
		if (health == 0) 
		{
			rend.material.SetColor("_Color", Color.black);
		}
		
		else
		{
			this.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, (health/setHealth), 1)); 
		}
	}
	
	public float getThickness()
	{
		return armourThickness;
	}
	
	public void giveDamage(float damage)
	{
		health -= damage;
		
		if (health < 0){ health = 0; }
	}
	
	public string getName()
	{
		return partName;
	}
}
