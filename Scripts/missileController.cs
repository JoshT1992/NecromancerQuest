/*
** When a missile object is created, this is attached to it. This let's the
** missile object behave correctly and detonate when it hits an enemy
** or a piece of terrain.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileController : MonoBehaviour {

	public int dmg;
	public bool isPiercing;
	public bool isCold;
	public int team;
	
	public List<int> effects;
	
	void Start () {
		effects = new List<int>();
	}
	
	void Update () {
		
	}
	
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("unit")) {
			if (other.gameObject.GetComponent<stats>().team!=team) {
				other.GetComponent<stats>().takeHit(dmg, isPiercing, isCold);
				Destroy(gameObject);
			}
		} else if (other.gameObject.CompareTag("terrain")) {
			Destroy(gameObject);
		}
	}
}