using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using System;
public class TankMovement : MonoBehaviour
{
    [SerializeField] TankInputs input;
    private Rigidbody rb;

    [SerializeField] WheelCollider FLwheel;
    [SerializeField] WheelCollider FRwheel;
    [SerializeField] WheelCollider RLwheel;
    [SerializeField] WheelCollider RRwheel;

    //movement data
    [SerializeField] private float steeringRange = 90f;
    [SerializeField] private float maxMotorPower = 1000f;
    [SerializeField] private float maxBrakePower = 400f;
    [SerializeField] private float wheelFriction = 200f;
    [SerializeField] private float maxSpeed = 100;

    private float currentTorque;
    private float currentMaxSpeed;

    public Vector3 debugSpeed;
    public float powerInput;

    private void Awake()
    {
        input = new TankInputs();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        currentTorque = maxMotorPower;
        currentMaxSpeed = maxSpeed;
    }

    private void Update()
    {
        Movement();
        speedLimiter(new WheelCollider[] {FLwheel, FRwheel, RLwheel, RRwheel});
    }

    private void Movement()
    {
        //getting the input from the player
        float xInput = input.getMovementAxis().x;
        float yInput = input.getMovementAxis().y;
        Vector3 currentVector = rb.velocity;
        float currentSpeed = transform.InverseTransformDirection(currentVector).z;
        debugSpeed = transform.InverseTransformDirection(currentVector);

        float power; 

        //first im going to deal with the steering rotating the 2 front wheels
        FLwheel.steerAngle = xInput * steeringRange;
        FRwheel.steerAngle = xInput * steeringRange;

        //this where we decide if its going to be neutral or pivot steering
        //this will work in similar way to old version combining both the x and y input

        //in this iteration of the movement system power is going to based off how far away the stick is from the origin

        //im going to use an equation of a circle to find the radius it would have if it were on a circle 
        // (x - h)^2 + (y - k)^2 = r^2 where h and k is the origin of the circle x and y respectively 
        //h = 0 k = 0 and the radius is 1

        float circleResult = Mathf.Pow(xInput, 2) + Mathf.Pow(yInput, 2);
        circleResult = Mathf.Sqrt(circleResult);
        power = circleResult;

        powerInput = power;

        //now im going to deal with what wheels get given power or brake force
        //this is based on the amount of steering input and whether its forward or backwards

        if (power == float.NaN)  //poweer returns nan when inpout is 0
        {
            allWheelPower(0, power, yInput);
        }

        //this checks if the speed is low and enough and steering input is high enouhg for it start pivot turning
        else if (Mathf.Abs(currentSpeed) < 5 & Mathf.Abs(xInput) > 0.8f)  //turns on brake and rotates more or less on the spot
        {
            pivotAllWheelPower(power, xInput);
        }

        else  //turns normally using the front wheels to steer so all i need to do is apply power
        {
            allWheelPower(power, currentSpeed, yInput);
        }
    }

    private void allWheelPower(float input, float speed, float yInput)
    {
        if (yInput < 0)
        {
            input = -input;
        }

        if (input == 0 || input == float.NaN)
        {
            FLwheel.brakeTorque = wheelFriction;
            FRwheel.brakeTorque = wheelFriction;
            RLwheel.brakeTorque = wheelFriction;
            RRwheel.brakeTorque = wheelFriction;

            FLwheel.motorTorque = 0;
            FRwheel.motorTorque = 0;
            RLwheel.motorTorque = 0;
            RRwheel.motorTorque = 0;

            Debug.Log("No Input!");
        }

        else if (input * speed < -1f)  //it needs to go in the opposite direction
        {
            FLwheel.brakeTorque = Mathf.Abs(input * maxBrakePower);
            FRwheel.brakeTorque = Mathf.Abs(input * maxBrakePower);
            RLwheel.brakeTorque = Mathf.Abs(input * maxBrakePower);
            RRwheel.brakeTorque = Mathf.Abs(input * maxBrakePower);

            Debug.Log("Opposite Input");

            FLwheel.motorTorque = 0;
            FRwheel.motorTorque = 0;
            RLwheel.motorTorque = 0;
            RRwheel.motorTorque = 0;
        }

        else if (input * speed >= 0) //this would mean its going the same direction
        {
            FLwheel.motorTorque = input * maxMotorPower;
            FRwheel.motorTorque = input * maxMotorPower;
            RLwheel.motorTorque = input * maxMotorPower;
            RRwheel.motorTorque = input * maxMotorPower;

            FLwheel.brakeTorque = 0;
            FRwheel.brakeTorque = 0;
            RLwheel.brakeTorque = 0;
            RRwheel.brakeTorque = 0;

            Debug.Log("Normal");
        }
    }

    private void pivotAllWheelPower(float input ,float steeringInput)
    {
        //first lets make whatever wheel is on the inside rear brake

        if (steeringInput < 0) //if user wants to go left
        {
            RLwheel.brakeTorque = Mathf.Abs(steeringInput) * maxBrakePower;
            RLwheel.motorTorque = 0;

            RRwheel.motorTorque = input * maxBrakePower;
            RRwheel.brakeTorque = 0;
        }
        else
        {
            RRwheel.brakeTorque = Mathf.Abs(steeringInput) * maxBrakePower;
            RRwheel.motorTorque = 0;

            RLwheel.motorTorque = input * maxBrakePower;
            RLwheel.brakeTorque = 0;
        }

        //then we apply power to the front wheels

        FLwheel.motorTorque = input * maxMotorPower;
        FRwheel.motorTorque = input * maxMotorPower;
        FLwheel.brakeTorque = 0;
        FRwheel.brakeTorque = 0;
    }

    private void speedLimiter(WheelCollider[] wheels)
    {
        foreach (WheelCollider wheel in wheels)
        {
            if (wheel.rotationSpeed > maxSpeed)
            {
                wheel.rotationSpeed = maxSpeed;
            }
            else if (wheel.rotationSpeed < -maxSpeed)
            {
                wheel.rotationSpeed = -maxSpeed;
            }
        }
    }

    public void setTorqueModifier(float mod)
    {
        currentTorque = maxMotorPower * mod;
    }

    public void setSpeedModifier(float mod)
    {
        currentMaxSpeed = maxSpeed * mod;
    }

    public float getEnginePerformance()
    {
        return currentTorque/maxMotorPower;
    }

    public float getSpeedPerformance()
    {
        return currentMaxSpeed/maxSpeed;
    }
}
