using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private float hForce;
	private float yForce;
	private float vForce;
	
	LineRenderer line;
	
	private Rigidbody rb;
	
	public GameObject selectionSphere;
	
	private GameObject zombie;
	
	public GameObject target;
	
	private float speed;

	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
		hForce = 0;
		yForce = 0;
		vForce = 0;
		
		line = gameObject.GetComponent<LineRenderer>();
		line.enabled = false;
		
		selectionSphere.SetActive(false);
		
		speed = gameObject.GetComponent<stats>().speed/2;
		
		//Screen.lockCursor = true;
	}
	
	void Update () {
		if (Input.GetKey("a"))
			hForce = -1.0f;
		if (Input.GetKey("d"))
			hForce = 1.0f;
		if (Input.GetKey("s"))
			vForce = -1.0f;
		if (Input.GetKey("w"))
			vForce = 1.0f;
		
		if (Input.GetMouseButton(0)) {
			selectionSphere.GetComponent<selectionControl>().selectionType = 0;
			StopCoroutine("SelectRay");
			StartCoroutine("SelectRay");
		} else if (Input.GetMouseButton(1)) {
			selectionSphere.GetComponent<selectionControl>().selectionType = 1;
			StopCoroutine("TargetRay");
			StartCoroutine("TargetRay");
		} else if (Input.GetKey("h")) {
			for (int i = 0; i < selectionSphere.GetComponent<selectionControl>().units.Count; i++) {
				selectionSphere.GetComponent<selectionControl>().units[i].GetComponent<AIController>().target = gameObject;
				selectionSphere.GetComponent<selectionControl>().units[i].GetComponent<AIController>().setOrders(1);
				selectionSphere.GetComponent<selectionControl>().units[i].GetComponent<AIController>().isSelected = false;
			}
			selectionSphere.GetComponent<selectionControl>().units.Clear();
		}
	}
	
	IEnumerator SelectRay() {
		line.enabled = true;
		
		while (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			line.SetPosition (0, ray.origin + Camera.main.transform.right);
			
			int layerMask = 1 << 8;
			
			if (Physics.Raycast(ray, out hit, 100, layerMask)) {
				line.SetPosition(1, hit.point);				
				selectionSphere.SetActive(true);
				selectionSphere.transform.position = hit.point;
			} else {
				line.SetPosition (1, ray.GetPoint(100));
			}
			
			yield return null;
		}
		
		line.enabled = false;
		selectionSphere.GetComponent<selectionControl>().objects.Clear();
		selectionSphere.SetActive(false);
	
	
	}
	
	IEnumerator TargetRay() {
		line.enabled = true;
		
		while (Input.GetMouseButton(1)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			line.SetPosition (0, ray.origin + Camera.main.transform.right);
			
			int layerMask = 1 << 8;
			
			if (Physics.Raycast(ray, out hit, 100, layerMask)) {
				line.SetPosition(1, hit.point);				
				selectionSphere.SetActive(true);
				selectionSphere.transform.position = hit.point;
			} else {
				line.SetPosition (1, ray.GetPoint(100));
			}
			
			yield return null;
		}
		
		line.enabled = false;
		selectionSphere.GetComponent<selectionControl>().objects.Clear();
		selectionSphere.SetActive(false);
	
	
	}
	
	void FixedUpdate () {
		Vector3 movement = new Vector3 (hForce, yForce, vForce);
	
		rb.velocity = transform.rotation*movement*speed;
		
		hForce = 0;
		yForce = 0;
		vForce = 0;
	}
}