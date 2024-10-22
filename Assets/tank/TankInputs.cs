using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankInputs
{
    Vector2 movementInput;

    TankControls tankControls;
    InputAction movementAxis;

    public TankInputs()
    {
        tankControls = new TankControls();

        movementAxis = tankControls.Tank.movement;

        movementAxis.Enable();
    }

    public Vector2 getMovementAxis()
    {
        return movementAxis.ReadValue<Vector2>();
    }
}
