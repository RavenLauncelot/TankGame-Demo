using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyObject : MonoBehaviour
{
    [SerializeField] private GameObject[] objectToBeDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool destroy = true;

        //this checks if all the objects are still within the scene 
        //if they all are null then destroy will stay as true and the second if will destroy this object
        foreach (var obj in objectToBeDestroyed)
        {
            if (obj != null)
            {
                destroy = false;
            }
        }

        if (destroy)
        {
            Destroy(this.gameObject);
        }
    }
}
