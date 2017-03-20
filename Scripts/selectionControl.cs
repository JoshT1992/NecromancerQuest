/*
** The selectionControl script handles the player's ability to select
** allies and enemies to deliver orders such as follow or attack.
** 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectionControl : MonoBehaviour {
	
	public List<GameObject> units;
	public List<GameObject> objects;
	
	public int selectionType;
	public GameObject player;

	void Start () {
		units = new List<GameObject>();
		objects = new List<GameObject>();
		selectionType = 0;
	}
	
	void Update () {
	
	}
	
	void OnTriggerEnter (Collider other) {
		bool isObjects = false;
		for (int i = 0; i < objects.Count; i++) {
			if (other.gameObject==objects[i])
				isObjects = true;
		}
		if (!isObjects)
			objects.Add(other.gameObject);
			
		if (other.gameObject.CompareTag("unit")) {
			if ((other.gameObject.GetComponent<stats>().team==1)&&(selectionType==0)) {
				bool isSelected = false;
				for (int i = 0; i < units.Count; i++) {
					if (units[i] == other.gameObject)
						isSelected = true;
				}
				if (!isSelected) {
					units.Add (other.gameObject);
					other.gameObject.GetComponent<AIController>().isSelected = true;
				}
			}else if (selectionType==1) {
				int ord = 0;
				
				if (other.gameObject.GetComponent<stats>().team==2) {
					ord = 2;
				} else {
					ord = 1;
				}
				for (int i = 0; i < units.Count; i++) {
					units[i].GetComponent<AIController>().target = other.gameObject;
					units[i].GetComponent<AIController>().setOrders(ord);
					units[i].GetComponent<AIController>().isSelected = false;
				}
				units.Clear();
				player.GetComponent<PlayerController>().target = other.gameObject;
			}
		} else if (selectionType==1) {
			if (other.gameObject.CompareTag("terrain")) {
				bool isUnits = false;
				
				for (int i = 0; i < objects.Count; i++) {
					if (objects[i].CompareTag("unit"))
						isUnits = true;
				}
				
				if (!isUnits) {
					for (int i = 0; i < units.Count; i++) {
						units[i].GetComponent<AIController>().targetLoc = gameObject.transform.position;
						units[i].GetComponent<AIController>().setOrders(3);
						units[i].GetComponent<AIController>().isSelected = false;
					}
					units.Clear();
				}
			}
		}
	}
	
	void OnTriggerExit (Collider other) {
		objects.Remove(other.gameObject);
	}
}