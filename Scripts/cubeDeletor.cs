using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeDeletor : MonoBehaviour {

	public bool broken;
	public Renderer rm;
	public Transform tm;

	// Use this for initialization
	void Start () {
		broken = false;
		rm = gameObject.GetComponent<Renderer>();
		tm = gameObject.GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (broken) {
			float x = rm.material.color.r;
			float y = rm.material.color.b;
			float z = rm.material.color.g;
			if (x == 0)
				x += 0.1f;
			if (x < 1)
				x *= 1.04f;
			if (y > 0)
				y *= 0.96f;
			if (z > 0)
				z *= 0.96f;
				
			rm.material.color = new Color (x,y,z,1);
			tm.localScale *= 0.99f;
		}
	}
	
	void OnTriggerEnter (Collider other) {
		if ((other.gameObject.CompareTag("terrain"))&&(broken)) {
			Destroy(gameObject);
		}
	}
}