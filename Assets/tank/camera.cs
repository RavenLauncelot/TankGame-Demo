using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class camera : MonoBehaviour
{
    public Transform normalPos;
    public Transform zoomInPos;

    Transform tf;
    public float camSpeed;

    private InputAction zoomIn;
    private TankControls controls;

    public bool zoom;

    void Awake()
    {
        controls = new TankControls();
    }

    void OnEnable()
    {
        zoomIn = controls.Tank.zoomin;
        zoomIn.performed += CamZoom;
        zoomIn.Enable();

        controls.Enable();
    }

    void OnDisable()
    {
        zoomIn.Disable();

        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        tf = this.GetComponent<Transform>();
    } 

    // Update is called once per frame
    void FixedUpdate()
    {
        if (zoom == false)
        {
            Vector3 currentPos = tf.position; 
            Vector3 newPos = normalPos.position;
            tf.position = Vector3.Lerp(currentPos, newPos, camSpeed * Time.deltaTime);

            Quaternion camAngle = normalPos.rotation;
            tf.rotation = camAngle;
        }

        else
        {
            Vector3 newPosition = zoomInPos.position;
           
            Quaternion camAngle = zoomInPos.rotation;

            tf.position = newPosition;
            tf.rotation = camAngle;
        }
    }

    void CamZoom(InputAction.CallbackContext obj)
    {
        zoom =! zoom;
    }
}
