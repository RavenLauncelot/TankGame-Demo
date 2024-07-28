using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
	public Transform pivot;
	
	public float range;
	public float turretSpeed;
	
	Transform TF;
	public Transform player;
	
	void Start()
	{
		TF = this.GetComponent<Transform>();
		
		StartCoroutine(aimAtPlayer());
	}
	
	IEnumerator aimAtPlayer()
	{
		while (true)
		{
			//Debug.Log("Current distance: " + Vector3.Distance(TF.position, player.position));
			
			while (Vector3.Distance(TF.position, player.position) < range)   //aim at player stop once is aimed at player and aim again after set time
			{
				Vector3 previousPlayerLoc = player.position;
				

				pivot.rotation = Quaternion.RotateTowards(Quaternion.look)
					
				previousPlayerLoc = player.position;
					
				RaycastHit hit;
				if (Physics.Raycast(pivot.transform.position, pivot.forward, out hit, range))
				{
					Debug.DrawRay(pivot.transform.position, pivot.forward * 100, Color.red);
					if (hit.collider.tag == "Player")
					{
						Debug.Log("Firing");
						Fire();
						yield return new WaitForSeconds(5);

					}
				}
					
				//Debug.Log("Aiming at player");
				yield return null;	
			}
			
			yield return null;
			//Debug.Log("Not in range");
		}
	}
	
	private void Fire()
	{
		//Debug.Log("Enemy turret fired");
	}
}
