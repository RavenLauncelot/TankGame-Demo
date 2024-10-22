using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firingCube : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSys;

    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject ricochetEffect;

    [SerializeField] private float penPower;
    [SerializeField] private float damage;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(firing());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)  //this is when the particle hits something
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
            }

            else
            {
                Instantiate(ricochetEffect, colEvents[i].intersection, Quaternion.LookRotation(Vector3.Reflect(colEvents[i].velocity.normalized, colEvents[i].normal)));
            }
        }
    }

    private IEnumerator firing()   //this just fires a shell evert specified seconds 
    {
        while (true)
        {
            particleSys.Play();
            yield return new WaitForSeconds(2f);
        }
    }
}
