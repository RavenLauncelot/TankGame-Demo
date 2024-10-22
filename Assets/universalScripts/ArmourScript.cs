using UnityEngine;

public class ArmourScript : MonoBehaviour
{
	//This script is going to be used on multiple differnet types of objects 
	//I did this so that i dont need to program my particlesystem to work with lots of different scripts
	
	
	[SerializeField] private float health;
	[SerializeField] private float armourThickness;
	[SerializeField] private string partName;

	private float initialHealth;

	Renderer rend;
	
	private void Start()
	{
		rend = this.GetComponent<Renderer>();
		initialHealth = health;
	}
	
	private void Update()
	{
		if (health <= 0) 
		{
			rend.material.SetColor("_Color", Color.black);
		}
		
		else
		{
			this.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, (health/initialHealth), 1)); 
		}
	}
	
	public float getThickness()
	{
		return armourThickness;
	}
	
	public void giveDamage(float damage)
	{
		health -= damage;
	}
	
	public string getName()
	{
		return partName;
	}

	public float getHealth()
	{
		return health;
	}
	
	public float getHealthPercent()
	{
		if (health < 0)
		{
			return 0;
		}
		else
		{
			return health / initialHealth;
		}
	}
}
