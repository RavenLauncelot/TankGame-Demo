using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class followCameraAngle : MonoBehaviour
{

    public float turretYawSpeed;
    public float turretPitchSpeed;

    public float minPitch;
    public float maxPitch;

    float currentGunPitch;

    public Transform camPivotY;
    public Transform camPivotX;

    public Transform gunPivotX;
    Transform gunPivotY;

    private void Start()
    {
        gunPivotY = this.GetComponent<Transform>();

        currentGunPitch = 0;
        gunPivotX.localEulerAngles = Vector3.zero;
    }

    private void FixedUpdate()  //well be doing it in fixed update since this object is connected to a rigidbody
    {
        gunPivotY.rotation = Quaternion.RotateTowards(gunPivotY.rotation, camPivotY.rotation, turretYawSpeed * Time.deltaTime);   //because ive split the axis into seperate gameobjects in the editor i dont need need to single out the axis i want to move 

        Quaternion movementInX = new Quaternion();
        movementInX.eulerAngles = new Vector3(Mathf.Clamp(camPivotX.localEulerAngles.x, minPitch, maxPitch), 0, 0);                   //y and z are going to be equal to 0 anyway so this shouldnt matter
        gunPivotX.localRotation = Quaternion.RotateTowards(gunPivotX.localRotation, movementInX, turretPitchSpeed * Time.deltaTime);

        Debug.Log(movementInX.eulerAngles);
    }
}
