using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingArmour : MonoBehaviour
{
    [SerializeField] ArmourScript[] armouredParts;

    [SerializeField] private float totalHealth;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float updatedHP = 0;

        foreach (ArmourScript armour in armouredParts)
        {
            updatedHP += armour.getHealth();
        }

        totalHealth = updatedHP;

        if (totalHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
