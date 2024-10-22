using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hitChanceGui : MonoBehaviour
{
    [SerializeField] private Transform gunPos;
    [SerializeField] private fireScript fireScript;

    [SerializeField] private Text targetInformation;   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        calcPotentialDmg();
    }

    //this function caluclations the penetration chance and the possible damage it could deal
    private void calcPotentialDmg()
    {
        //resetting all values to zero or to values that don't change when aiming at something different
        float armourThickness = 0;
        float penPower = fireScript.getPenPower();
        float potentialDamage = 0;
        float penChance = 0;
        float shellDamage = fireScript.getDmg();

        //going to send out a ray from the gun to calculate its chance of penetrating
        RaycastHit hit;
        Ray ray = new Ray(gunPos.position, gunPos.forward);

        Physics.Raycast(ray, out hit);

        //if its hit nothing
        if (hit.collider == null)
        {
            
        }

        //checks if its hit an amour script so it has hit armour
        else if (hit.collider.TryGetComponent<ArmourScript>(out ArmourScript armScript) == true)
        {
            armourThickness = armScript.getThickness();           
            
            //from here we will use the same calulations the gun uses and then return this

            //this finds the angle of the ray and the surface normal it has hit
            float angleOfHit = Vector3.Angle(gunPos.forward, -hit.normal);
            

            //this turns the angle of hit into a mutilpier for the hit chance. lower the angle the higher chance.
            float angleFactor = (-1f / 90f * angleOfHit) + 1f;

            //considering the armour thickness and the pentration values to find the final penetration chance
            penChance = (angleFactor * penPower) / armourThickness;

            potentialDamage = (penPower / armourThickness) * shellDamage;
        }

        //if its hit something but i cant be dealt damage
        else if (hit.collider != null)
        {
            
        }

        updateGUIData(penChance, penPower, potentialDamage, shellDamage, armourThickness);
    }

    private void updateGUIData(float penChanceGUI, float penPowerGUI ,float damage, float shellDamageGUI, float targetArmourGUI)
    {
        //this will update all the values on the GUI
        targetInformation.text = "Target Data:\nArmour thickness: " + targetArmourGUI + "\nPotential damage: " + damage + "\n\nShell information:\n" + "Shell damage: " + shellDamageGUI + "\nPenetration Power: " + penPowerGUI + "\nPenetration chance: " + penChanceGUI*100 + "%";
    }
}
