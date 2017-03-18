using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class signController : MonoBehaviour {

	public Text txt;
	
	private int autoDelete;

	void Start () {
		autoDelete = 0;
	}
	
	void FixedUpdate () {
		autoDelete++;
		if (autoDelete > 1000)
			Destroy(gameObject);
	}
}
