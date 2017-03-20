/*
** Handles the ability for AI to see other AI. Currently the player is
** "invisible" to them but this will be changed eventually.
**
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vision : MonoBehaviour {

	public List<GameObject> creatures;
	
	void Start () {
		creatures = new List<GameObject>();
	}
	
	void Update () {
		for (int i = 0; i < creatures.Count; i++) {
			if (creatures[i] == null)
				creatures.Remove(creatures[i]);
		}
	}
	
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("unit")) {
			bool isSeen = false;
			for (int i = 0; i < creatures.Count; i++) {
				if (other.gameObject==creatures[i])
					isSeen = true;
			}
			if (!isSeen)
				creatures.Add(other.gameObject);
		}
	}
	
	void OnTriggerExit (Collider other) {
		if (other.gameObject.CompareTag("unit"))
			creatures.Remove(other.gameObject);
	}
}
