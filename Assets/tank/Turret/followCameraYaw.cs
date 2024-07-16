using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class followCameraYaw : MonoBehaviour
{
    Transform TF;
    public float turretYawSpeed;

    public Transform camPivotY;

    private void Start()
    {
        TF = this.GetComponent<Transform>();
    }

    private void FixedUpdate()  //well be doing it in fixed update since this object is connected to a rigidbody
    {
        Quaternion movementStep = Quaternion.Lerp(TF.rotation, camPivotY.rotation, turretYawSpeed * Time.deltaTime);
                                                               
        TF.rotation = movementStep;   //because ive split the axis into seperate gameobjects in the editor i dont need need to single out the axis i want to move 
    }
}
