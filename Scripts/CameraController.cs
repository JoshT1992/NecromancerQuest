/*
** The Camera Controller handles all aspects of the camera. It is generally
** set to follow the player and move based on cursor movements. Currently
** it is extremely basic and mostly just made to have something working for
** debugging/testing.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	
	public GameObject player;
	
	private Vector2 mousePos;
	
	private float dx;//, dy;
	
	private float rSpeed = 0.2f;
	
	private Vector3 offset;

	private float leftSide = Screen.width*0.15f;
	private float rightSide = Screen.width*0.85f;


	//private bool resetCurs;
	//private bool leftTurn;
	//private bool rightTurn;

	void Start () {
		mousePos = Input.mousePosition;
		Cursor.lockState = CursorLockMode.Confined;
		
		transform.LookAt(player.transform.position);

		dx = 0.0f;
		offset = transform.position - player.transform.position;

		//resetCurs = false;

		//leftTurn = false;
		//rightTurn = false;
	}
	
	void Update() {
		//Grabbing the x position of the mouse and comparing 
		// to the left or right of the screen
		float xVal = mousePos.x;
		dx = 0;
		if (xVal<=leftSide){
			dx = 0 - ((leftSide - xVal)/7.5f * rSpeed);

		}
		if (xVal>=rightSide){
			dx = ((xVal - rightSide)/7.5f) * rSpeed;
		}

		mousePos = Input.mousePosition;
		/*
		if (Input.GetKeyDown("space")){
			resetCurs = true;
			dx = 0;
		}
		if (resetCurs == true){
			if(Input.GetKeyUp("space")){
				resetCurs = false;
				mousePos = Input.mousePosition;
			}
		}else{
			dx = -(mousePos.x - Input.mousePosition.x)*rSpeed;
			//dy = mousePos.y - Input.mousePosition.y;

			mousePos = Input.mousePosition;
		}
		*/
		
	}
	
	void LateUpdate() {
		offset = Quaternion.AngleAxis (dx, Vector3.up) * offset;
		transform.position = player.transform.position + offset;
		transform.LookAt(player.transform.position);
		player.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		
	}
}