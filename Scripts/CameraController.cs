using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	
	public GameObject player;
	
	private Vector2 mousePos;
	
	private float dx;//, dy;
	
	private float rSpeed = 1.0f;

	private bool resetCurs;
	
	private Vector3 offset;

	void Start () {
		mousePos = Input.mousePosition;
		Cursor.lockState = CursorLockMode.Confined;
		
		transform.LookAt(player.transform.position);
		
		resetCurs = false;

		offset = transform.position - player.transform.position;
	}
	
	void Update() {
		if (Input.GetKeyDown("space")){
			resetCurs = true;
		}
		if (resetCurs == true){
			if(Input.GetKeyUp("space")){
				resetCurs = false;
				dx = 0;
				mousePos = Input.mousePosition;
			}
		}else{
			dx = -(mousePos.x - Input.mousePosition.x)*rSpeed;
			//dy = mousePos.y - Input.mousePosition.y;

			mousePos = Input.mousePosition;
		}
		
	}
	
	void LateUpdate() {
		offset = Quaternion.AngleAxis (dx, Vector3.up) * offset;
		transform.position = player.transform.position + offset;
		transform.LookAt(player.transform.position);
		player.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		
	}
}