using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OurCamera : MonoBehaviour {

	//What's our camera?
	public Camera theCamera;
	public float speed = 30f;
	public float speed_default = 30f;
    public float cameraSpeed = .025f;
	public int minZoom = 1;
	public int maxZoom = 50;
    //public static float border_width_max = .95f;        //What is the amount of border before we start to scroll max.
    public static float border_width_max = .99f;		//What is the amount of border before we start to scroll max.
    //public static float border_width_min = .05f;        //What is the amount of border before we start to scroll min.    public static float border_width_min = .05f;		//What is the amount of border before we start to scroll min.
    public static float border_width_min = .01f;		//What is the amount of border before we start to scroll min.


    public bool	overMenu = false;						//If we're over a menu item then we don't want to move the camera
	public static bool unitSelected = false;					//If this is false we'll leave unit dependant buttons greyed out
	public static bool scrollDisabled = false;
    public GameObject hexSelector;
    public GameObject cancelIcon;
    public GameObject chopIcon;



    //Screen edge detection
    [HideInInspector] public static float top_edge = 10;
	[HideInInspector] public static float bottom_edge = -12;
	[HideInInspector] public static float left_edge = -21;
	[HideInInspector] public static float right_edge = 21;

	public float scroll_fast_multiplier = 2f;


	// Use this for initialization
	void Awake () {

		theCamera =	Camera.main;

		//Init the camera goodness
		// top_edge = Screen.height * border_width_max;
		// bottom_edge = Screen.height * (border_width_min + .02f);
		// left_edge = Screen.width * border_width_min;
		// right_edge = Screen.width * border_width_max;

        //transform.position = Camera.main.WorldToScreenPoint( new Vector2( 50, 50 ) );
        //gameObject.transform.position = Camera.main.ViewportToScreenPoint( new Vector2( 50, 50 ) );
    }

    // Update is called once per frame
    void Update () {

		//Move the Camera w/Mouse or Keyboard
		MoveCamera();
		//Enable Camera Zoom
		ZoomCamera ();
	}

	public void EnableMenu(bool _enableMenu) {
		if (_enableMenu) {
			unitSelected = true;
			Debug.Log("Enable Menu True!");
		}
		else {
			unitSelected = false;
			Debug.Log("Enable Menu False!");
		}
	}


	public void LockCamera(bool lockCam) {			//This Function will lock the camera so it doesn't scroll
		if (lockCam) {
			overMenu = true;
		}
		else {
			overMenu = false;
		}
	}

	void ZoomCamera() {
		//Zoom in and out
		// Scroll forward
		if (Input.GetAxis("Mouse ScrollWheel") > 0){
			ZoomOrthoCamera(theCamera.ScreenToWorldPoint(Input.mousePosition),5);
		}
		// Scoll back
		if (Input.GetAxis("Mouse ScrollWheel") < 0){
			ZoomOrthoCamera(theCamera.ScreenToWorldPoint(Input.mousePosition), -5);
		}
	}

	void ZoomOrthoCamera(Vector3 zoomTowards, float amount)	{
		// Calculate how much we will have to move towards the zoomTowards position
		float multiplier = (1.0f / theCamera.orthographicSize * amount);
		//Debug.Log("Ortho Size: " + theCamera.orthographicSize);

		//If the size is 1 we don't want to keep moving the mouse, it's annoying as shit
		if ( theCamera.orthographicSize > 1 && theCamera.orthographicSize < 50) {
			// Move camera
			transform.position += (zoomTowards - transform.position) * multiplier;
		}

		// Zoom camera
		theCamera.orthographicSize -= amount;

		// Limit zoom
		theCamera.orthographicSize = Mathf.Clamp(theCamera.orthographicSize, minZoom, maxZoom);
	}


	protected void MoveCamera () {
		//If Shift's being hit then Multiply by 4
		if (Input.GetKey (KeyCode.LeftShift)  ) {
			speed = speed_default * scroll_fast_multiplier;
		} else {
			speed = speed_default;
		}


		GameObject _Cam = Camera.main.gameObject;

		if ( (Camera.main.transform.position.y <= top_edge && Input.GetKey(KeyCode.W))   ) {
			transform.Translate(Vector3.up * cameraSpeed * speed, Space.World);
		}
		if ( (Camera.main.transform.position.y >= bottom_edge && Input.GetKey(KeyCode.S))  ) {
			transform.Translate(Vector3.down * cameraSpeed * speed, Space.World);
		}
		if ( (Camera.main.transform.position.x <= right_edge && Input.GetKey(KeyCode.D))  ) {
			transform.Translate(Vector3.right * cameraSpeed * speed, Space.World);
		}
		if ( (Camera.main.transform.position.x >= left_edge && Input.GetKey(KeyCode.A)) ) {
			transform.Translate(Vector3.left * cameraSpeed * speed, Space.World);
		}
		//Debug.Log("Cam Inside? " +CamInsideBounds(_Cam));

	}



}
