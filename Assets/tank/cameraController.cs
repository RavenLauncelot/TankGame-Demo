using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class cameraController : MonoBehaviour
{
	Transform pivotY;
	public Transform pivotX;  //this is a child of the pivotY where this script is located

	private InputAction zoomIn;
	private InputAction turretInput;
	private TankControls controls;

	[SerializeField] private GameObject scopedCameraObj;
	[SerializeField] private GameObject normalCameraObj;

	private Camera scopeCam;
	private Camera normalCam;
	private AudioListener scopeCamAudio;
	private AudioListener normalCamAudio;

	private Vector2 turretVec;

	private bool thirdPerson;
	private float currentCamPitch;

	public float camSpeedX;      //this is basically sensitivity 
	public float camSpeedY;
	public float maxCamPitch;   //this is so the camera wont do backflips or front flips
	public float minCamPitch;

	void Awake()
	{
		controls = new TankControls();
	}

	void OnEnable()
	{
		zoomIn = controls.Tank.zoomin;
		turretInput = controls.Tank.turret;
		
		zoomIn.performed += camZoom;
		zoomIn.Enable();
		turretInput.Enable();

		controls.Enable();
	}

	void OnDisable()
	{
		zoomIn.Disable();

		controls.Disable();
	}

	private void Start()
	{
		scopeCam = scopedCameraObj.GetComponent<Camera>();
		normalCam = normalCameraObj.GetComponent<Camera>();

		pivotY = this.GetComponent<Transform>();

		pivotY.localEulerAngles = Vector3.zero;
		pivotX.localEulerAngles = Vector3.zero;
		currentCamPitch = 0;

		scopeCam.enabled = false;
		normalCam.enabled = true;
		thirdPerson = true;
	}

	private void Update()
	{
		
	}

	private void FixedUpdate()
	{
		//updating the turret input
		turretVec = turretInput.ReadValue<Vector2>();

		//check what camera mode its in 
		if (thirdPerson)
		{
			Vector3 turretCamY = new Vector3(0, 0, 0);
			turretCamY.y = turretVec.x * camSpeedX * Time.deltaTime;
			pivotY.Rotate(turretCamY);

			Vector3 turretCamX = new Vector3(0, 0, 0);
			turretCamX.x = turretVec.y * camSpeedY * Time.deltaTime;
			turretCamX.x = Mathf.Clamp(turretCamX.x, minCamPitch - currentCamPitch, maxCamPitch - currentCamPitch);
			pivotX.Rotate(turretCamX);

			currentCamPitch += turretCamX.x;
		}

		else
		{
			//the gun will now not follow the camera and use the input from the mouse
			//will handle ui elements while camera is fixed to the gun
		}

		//use the input for the turret in here instead 
		//let it control the camera which the turret will then follow instead
	}

	private void camZoom(InputAction.CallbackContext obj)
	{
		scopeCam.enabled = !scopeCam.enabled;              //will just invert whether they are enabled or not one will be set to false when the program starts
		scopeCamAudio.enabled = !scopeCamAudio.enabled;    //this applies aswell to the audio listener
		normalCam.enabled = !normalCam.enabled;
		normalCamAudio.enabled = !normalCamAudio.enabled;

		thirdPerson = !thirdPerson;
	}
}
