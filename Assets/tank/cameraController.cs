using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class cameraController : MonoBehaviour
{
	Transform pivotY;         //this is the pivots own transform
	public Transform pivotX;  //this is a child transform of pivotY
	Transform cameraTF;       //this is the camera itself

	private InputAction zoomIn;
	private InputAction turretInput;
	private TankControls controls;

	[SerializeField] private GameObject scopedCameraObj;
	[SerializeField] private GameObject normalCameraObj;

	private Camera scopeCam;    //these are so i can enable and disable the cameras depending on when im using them
	private Camera normalCam;
	private AudioListener scopeCamAudio;
	private AudioListener normalCamAudio;

	private Vector2 turretVec;

	private bool thirdPerson;
	private float currentCamPitch;

	[SerializeField] private float camSpeedX;      //this is basically sensitivity 
	[SerializeField] private float camSpeedY;
	[SerializeField] private float maxCamPitch;   //this is so the camera wont do backflips or front flips
	[SerializeField] private float minCamPitch;
	
	float maxDistance;   //this is the max distance from the pivot point. this does not hold value until the game starts. its set based of its current positon in the editor

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
		scopeCamAudio = scopeCam.GetComponent<AudioListener>();
		normalCamAudio = normalCam.GetComponent<AudioListener>();

		pivotY = this.GetComponent<Transform>();

		pivotY.localEulerAngles = Vector3.zero;
		pivotX.localEulerAngles = Vector3.zero;
		currentCamPitch = 0;

		scopeCam.enabled = false;
		scopeCamAudio.enabled = false;
		normalCam.enabled = true;
		thirdPerson = true;
		
		cameraTF = normalCam.gameObject.GetComponent<Transform>();
		maxDistance = cameraTF.localPosition.z;
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
			thirdPersonCamera();
		}

		else
		{
			//the gun will now not follow the camera and use the input from the mouse/controller
			//will handle ui elements while camera is fixed to the gun
			
			
		}
	}

	private void camZoom(InputAction.CallbackContext obj)
	{
		scopeCam.enabled = !scopeCam.enabled;              //will just invert whether they are enabled or not one will be set to false when the program starts
		scopeCamAudio.enabled = !scopeCamAudio.enabled;    //this applies aswell to the audio listener
		normalCam.enabled = !normalCam.enabled;
		normalCamAudio.enabled = !normalCamAudio.enabled;

		thirdPerson = !thirdPerson;
	}
	
	public float getCamXAngle()
	{
		return currentCamPitch;	
	}
	
	public void thirdPersonCamera()
	{
		Vector3 turretCamY = new Vector3(0, 0, 0);
		turretCamY.y = turretVec.x * camSpeedX * Time.deltaTime;
		pivotY.Rotate(turretCamY);

		Vector3 turretCamX = new Vector3(0, 0, 0);
		turretCamX.x = turretVec.y * camSpeedY * Time.deltaTime;
		turretCamX.x = Mathf.Clamp(turretCamX.x, minCamPitch - currentCamPitch, maxCamPitch - currentCamPitch);
		pivotX.Rotate(turretCamX);

		currentCamPitch += turretCamX.x;
			
		//this will shoot a ray backward where the camera is. if its hits a object it will move the camera to where it has hit so it doesnt phase through objects 
		RaycastHit hit;
		Ray ray = new Ray(pivotX.position, -pivotX.forward);
		Debug.DrawRay(pivotX.position, pivotX.forward * maxDistance, Color.red);  //the ray comes from pivotX as that is the final axis of movement - maxdistance is already negative so im using a forward vector. max distance is the distance from the cameras pivot
			
		if (Physics.Raycast(ray, out hit, -maxDistance))  //if the camera is going to phase through an object
		{
			cameraTF.position = hit.point;
		}
		else
		{
			cameraTF.localPosition = new Vector3(0, 0, maxDistance);
		}
	}
	
	public Vector3 getTarget()
	{
		RaycastHit hit;
		Ray ray = new Ray(cameraTF.position, cameraTF.forward);
		Debug.DrawRay(cameraTF.position, cameraTF.forward * 100, Color.blue);
		
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			Debug.Log("hit target: " + hit.point);
			return hit.point;		
		}
		else
		{
			Debug.Log("Target not hit");
			return ray.GetPoint(2000);
		}
	}
}
