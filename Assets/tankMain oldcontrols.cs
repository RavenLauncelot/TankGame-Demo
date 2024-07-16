using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class tankMainoldcontrols : MonoBehaviour
{

    public TankControls controls;  //geting the script generated from the unity input system

    //setting up the individual control inputs
    private InputAction movement;
    private InputAction turret;
    private InputAction fire;

    //these will hold the values of the inputs during the update method alot shorter than left.readvalue<float>() so makes it a bit more readable and it will only need to get the input once everyframe rather than whenever its used.
    public Vector2 movementVecIn;
    public Vector2 turretVecIn;
    public bool fireIn;

    //getting all the necesarray objects and components that make up the tank so that i can access all their properties
    public GameObject TankTop ,TankSideR ,TankSideL ,TankRearA ,TankRearB ,TankFrontA ,TankFrontB ,TankTrackL ,TankTrackR;
    public Transform TurretPivot;
    public Transform GunPivot;

    private WheelCollider[] leftTrack;
    private WheelCollider[] rightTrack;

    Rigidbody RB;
    Transform TF;

    //set variables for things like speed  
    public float maxTurretSpeed = 10f;
    public float maxGunSpeed = 10f;

    //public float maxTurnSpeed = 25; 
    public float maxSpeed = 50;
    public float initialTorque = 50;
    public float breakingTorque = 70;

    //debug public variables
    public float currentTurretRotation;
    public float currentGunPitch;
    public float currentSpeed;

    public float rTrackTorque;
    public float lTrackTorque;
    public float rBrakingTorque;
    public float lBrakingTorque;
    public float rWheelSpeed;
    public float lWheelSpeed;

    void Awake(){
        controls = new TankControls();

        //TankTrackL ,TankTrackR.GetComponent<Rigidbody>().
    }

    void OnEnable(){
        //enabling each control for the tank 
        movement = controls.Tank.movement;
        turret = controls.Tank.turret;
        fire = controls.Tank.fire;

        movement.Enable();
        turret.Enable();
        fire.Enable();

        RB = this.GetComponent<Rigidbody>();
        TF = this.GetComponent<Transform>();
    }

    void OnDisable(){
        controls.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        //setting up the wheel colliders 
        leftTrack = TankTrackL.GetComponentsInChildren<WheelCollider>();
        rightTrack = TankTrackR.GetComponentsInChildren<WheelCollider>();
    }

    // Update is called once per frame
    void Update(){
        //updating debug variables
        currentGunPitch = GunPivot.localEulerAngles.x;
        currentTurretRotation = TurretPivot.localEulerAngles.y;
        currentSpeed = Vector3.Dot(transform.forward, RB.velocity);

        rTrackTorque = rightTrack[0].motorTorque;
        lTrackTorque = leftTrack[0].motorTorque;
        rBrakingTorque = rightTrack[0].brakeTorque;
        lBrakingTorque = leftTrack[0].brakeTorque;
        rWheelSpeed = rightTrack[0].rotationSpeed;
        lWheelSpeed = leftTrack[0].rotationSpeed;

        //getting inputs
        turretVecIn = turret.ReadValue<Vector2>();
        movementVecIn = movement.ReadValue<Vector2>();
        fireIn = fire.ReadValue<bool>();
    }

    void FixedUpdate(){
        //doing movement for the turret 

        Vector3 turretYaw = new Vector3(0 ,0 ,0);
        turretYaw.y = turretVecIn.x * maxTurretSpeed * Time.deltaTime;
        TurretPivot.Rotate(turretYaw);

        Vector3 turretPitch = new Vector3(0 , 0 ,0);
        turretPitch.x = turretVecIn.y * maxTurretSpeed * Time.deltaTime;
        GunPivot.Rotate(turretPitch);

        //doing movement for the tank itself

        //old movement -----------

        //this will be for the forward and backward movement. ill be using velocity so i will have a finer control on its speed and acceleration
        /*/
        float targetspeed = maxSpeed * movementVecIn.y;

        if (targetspeed >= RB.velocity.z){    //tank is accelerating
            Vector3 targetVector = TF.forward * targetspeed;
            RB.velocity = Vector3.Lerp(RB.velocity, targetVector , acceleration * Time.deltaTime);
        }

        else if (targetspeed < RB.velocity.z && targetspeed < 0){  //tank is accelerating backwards
            Vector3 targetVector = TF.forward * targetspeed;
            RB.velocity = Vector3.Lerp(RB.velocity, targetVector , acceleration * Time.deltaTime);
        }

        else{         //tank is decelerating
            Vector3 targetVector = TF.forward * targetspeed;
            RB.velocity = Vector3.Lerp(RB.velocity,targetVector, deceleration * Time.deltaTime);
        }

        float turnSpeed = movementVecIn.x * maxTurnSpeed * Time.deltaTime;
        TF.Rotate(new Vector3(0 ,turnSpeed ,0));
        /*/

        //end of old movement ---------------

        if (-0.4 < movementVecIn.y && movementVecIn.y < 0.4)       //within rotating zone tank is rotating
        {
            if (movementVecIn.x > 0)  //turning right
            {
                trackMovement(leftTrack, movementVecIn.x);
                trackMovement(rightTrack, -movementVecIn.x);
            }

            else  //turning left
            {
                trackMovement(rightTrack, -movementVecIn.x);   //movementvecin will be negative so needs to be positive
                trackMovement(leftTrack, movementVecIn.x);
            }
        }

        else  //out of rotating zone tank is driving and turning
        {
            if (movementVecIn.x > 0) //turning right
            {
                trackMovement(rightTrack, movementVecIn.y * (1 - movementVecIn.x)); //inside slower moving track
                trackMovement(leftTrack, movementVecIn.y);
            }

            else  //turning left
            {
                trackMovement(leftTrack, movementVecIn.y * (1 + movementVecIn.x));  //inside slower moving track  in this the movementvecin.x is negative so needs to be added instead
                trackMovement(rightTrack, movementVecIn.y);
            }
        }

        //this code for the movement does not work by itself as the amount of torque can change depending on speed. for example when the tank is at full speed the torque should be set to 0 so that it doesnt accelerate anymore
        //due to their being a lot of wheels im going to be dealing with the speed of each wheel in the track movement function       
    }


    private void trackMovement(WheelCollider[] wheels, float input)
    {
        //float wheelSpeed;
        float wheelSpeed;
 
        if (input == 0)   //tank is trying to brake
        {
            foreach(WheelCollider a in wheels)  //in this foreach we will decide whether the wheel is stationary going forward in reverse.  this will effect how much torque we give to each wheel
            {
                //wheelSpeed = a.rotationSpeed;
                a.motorTorque = 0;
                a.brakeTorque = breakingTorque * 0.8f;   //when no input is given it will put on the brakes a little bit so it can still rotate but wont suddently stop when trying to coast
            }
        }

        else  //tank is trying to accelerate or brake in the opposite direction
        {           
            foreach(WheelCollider a in wheels)
            {
                wheelSpeed = a.rotationSpeed;
                if (wheelSpeed * input >= 0)    //if they share the same operator ,so they are both in same direction ,the result will be above zero.
                {
                    if (input > 0)
                    {
                        a.brakeTorque = 0;
                        a.motorTorque = Mathf.Lerp(initialTorque, 0, wheelSpeed/(maxSpeed*input));
                    }
                    else
                    {
                        a.brakeTorque = 0;
                        a.motorTorque = Mathf.Lerp(-initialTorque, 0, wheelSpeed/(maxSpeed*input));
                    }
                    
                }

                else  //in this case they dont share the same operator so the user wants to go the oposite direction the tank is going so it will brake
                {
                    if (input > 0)
                    {
                        a.motorTorque = 0;
                        a.brakeTorque = breakingTorque * input;
                    }
                    else
                    {
                        a.motorTorque = 0;
                        a.brakeTorque = breakingTorque * -input;
                    }
                    
                }
            }
        }
    }
}

 
