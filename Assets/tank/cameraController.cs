using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class cameraController : MonoBehaviour
{
	Transform pivotY;         //this is the pivots own transform
	public Transform pivotX;  //this is a child transform of pivotY
	Transform cameraTF;       //this is the camera itself
	GameObject crosshair;     //this is crosshair gameobject
	
	//inputactions these are for the controls
	private InputAction zoomIn;
	private InputAction turretInput;
	private TankControls controls;

	//camera objects so i can turn them off
	[SerializeField] private GameObject scopedCameraObj;
	[SerializeField] private GameObject normalCameraObj;

	//these are the camera objects components
	private Camera scopeCam;    
	private Camera normalCam;
	private AudioListener scopeCamAudio;
	private AudioListener normalCamAudio;

	//the variable used for input this is updated every fixed update
	private Vector2 turretVec;

	//these are for the camera mode its in. and campitch is used for keeping track of its pitch to stay within its limits
	private bool thirdPerson;
	private float currentCamPitch;

	[SerializeField] private float camSpeedX;      //speed of the camera
	[SerializeField] private float camSpeedY;
	[SerializeField] private float maxCamPitch;   //these are the limits for the cameras rotation 
	[SerializeField] private float minCamPitch;   //stop the camera from doing loops 
	
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
		crosshair = GameObject.Find("crosshair");

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

		//check what camera mode its in. this is change in the camZoom method which is subscribed to an input action
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
		//inversing the states of each camera 
		scopeCam.enabled = !scopeCam.enabled;              //will just invert whether they are enabled or not one will be set to false when the program starts
		scopeCamAudio.enabled = !scopeCamAudio.enabled;    //this applies aswell to the audio listener
		normalCam.enabled = !normalCam.enabled;
		normalCamAudio.enabled = !normalCamAudio.enabled;

		//changing the third person bool
		thirdPerson = !thirdPerson;
		
		//making the crosshair appear or dissapear
		crosshair.SetActive(thirdPerson);  //will be active when third person is active
		
		//canvas stuff - changing the ui depending on the view
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
			
		if (Physics.Raycast(ray, out hit, -maxDistance))  //if the camera is going to phase through an object
		{
			cameraTF.position = hit.point;  //this sets the cameras positon to where the ray hits the ground 
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
			return hit.point;		
		}
		else
		{
			return ray.GetPoint(2000);
		}
	}
	
	public bool isThirdPerson()
	{
		return thirdPerson;
	}
}
