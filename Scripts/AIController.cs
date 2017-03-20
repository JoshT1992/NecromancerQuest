/*
The AI Controller manages all aspects of any non-player controlled units
The AI is given a set of tasks that it must accomplish. The tasks can
received either through orders from the player or through it's own
low level intelligence.




*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

	public GameObject target;
	public Vector3 targetLoc;
	
	public bool isSelected;
	public List<int> orders;
	
	public bool inRange;
	
	public GameObject vision;
	
	public abilityController[] abilities;
	
	public UnityEngine.AI.NavMeshAgent nav;
	
	public GameObject speechBubble;
	private bool lockSpeech;
	
	private int reCalculator;

	void Start () {
		vision = gameObject.transform.GetChild(0).gameObject;
		isSelected = false;
		inRange = false;
		nav = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		nav.speed = gameObject.GetComponent<stats>().speed/2;
		orders = new List<int>();
		lockSpeech = false;
		reCalculator = 100;
		
		abilities = gameObject.GetComponents<abilityController>();
	}
	
	public void setOrders (int order) {
		orders.Clear();
		orders.Add (order);
		inRange = false;
		reCalculator = 100;
	}
	
	void Update() {
		nav.speed = gameObject.GetComponent<stats>().speed/2;
		
		if (gameObject.GetComponent<stats>().slowPercent > 0)
			say ("I was slowed today!");
		
		if (target == null) {
			target = gameObject;
			setOrders (0);
		}
			
		if (isSelected) {
			gameObject.GetComponent<Renderer>().material.SetColor ("_OutlineColor", Color.red);
		} else {
			gameObject.GetComponent<Renderer>().material.SetColor ("_OutlineColor", Color.blue);
		}
		
		if (orders[0]==0) { //No orders, decide for themselves
			if (nearEnemy()) { //Attack nearest enemy
				//say ("I found an enemy!");
				target = nearestEnemy();
				setOrders(2);
			} else if (nearAlly()) { //Follow nearest Ally
				//say ("I found a friend!");
				target = nearestAlly();
				setOrders(1);
			}
		}
		
		if (orders[0]==1) { //Follow target
			moveTo(target.transform.position, 10.0f);
			//say ("I'm following!");
		}
		
		if (orders[0]==2) { //Attack target
			moveTo(target.transform.position, gameObject.GetComponent<stats>().range);
			if (inRange)
				attack();
			//say ("I'm attacking!");
		}
		
		if (orders[0]==3) { //Move to
			moveTo (targetLoc, 0.0f);
			//say ("I'm moving!");
		}
		
		if (Vector3.Distance (gameObject.transform.position, target.transform.position) <= gameObject.GetComponent<stats>().range)
			inRange = true;
		else
			inRange = false;
		
		if (GetComponent<stats>().isDead()) {
			Destroy(gameObject);
		}
		
		reCalculator++;
	}
	
	void moveTo (Vector3 location, float minDist) {
		if (reCalculator >= 100) {
			if (((gameObject.transform.position - location).sqrMagnitude) > minDist) {
				nav.destination = location;
				nav.stoppingDistance = minDist;
			} else if (orders[0]==3) {
				setOrders(0);
			}
		}
	}
	
	void say (string words) {
		if (!lockSpeech)
			StartCoroutine("says", words);
	}
	
	IEnumerator says (string words) {
		if (speechBubble!=null) {
			lockSpeech = true;
			GameObject sb = Instantiate (speechBubble) as GameObject;
			sb.transform.position = new Vector3 (gameObject.transform.position.x,gameObject.transform.position.y+20,gameObject.transform.position.z);
			sb.GetComponent<signController>().txt.text = words;
			
			yield return new WaitForSeconds (5.0f);
			
			Destroy(sb);
			lockSpeech = false;
		}
	}
	
	bool nearEnemy () {
		for (int i = 0; i < vision.GetComponent<vision>().creatures.Count; i++) {
			if (vision.GetComponent<vision>().creatures[i].GetComponent<stats>().team!=gameObject.GetComponent<stats>().team) {
				return true;
			}
		}
		return false;
	}
	
	bool nearAlly () {
		for (int i = 0; i < vision.GetComponent<vision>().creatures.Count; i++) {
			if (vision.GetComponent<vision>().creatures[i].GetComponent<stats>().team==gameObject.GetComponent<stats>().team) {
				return true;
			}
		}
		return false;
	}
	
	GameObject nearestEnemy () {
		GameObject closest = gameObject;
		int firstCheck = 0;
		
		for (int i = 0; i < vision.GetComponent<vision>().creatures.Count; i++) {
			if (vision.GetComponent<vision>().creatures[i].GetComponent<stats>().team!=gameObject.GetComponent<stats>().team) {
				closest = vision.GetComponent<vision>().creatures[i];
				firstCheck = i+1;
				break;
			}
		}
		
		float dist = Vector3.Distance (gameObject.transform.position, closest.transform.position);
		
		for (int i = firstCheck; i < vision.GetComponent<vision>().creatures.Count; i++) {
			if ((vision.GetComponent<vision>().creatures[i].GetComponent<stats>().team!=gameObject.GetComponent<stats>().team)&&(Vector3.Distance(gameObject.transform.position, vision.GetComponent<vision>().creatures[i].transform.position) < dist)) {
				dist = Vector3.Distance (gameObject.transform.position, vision.GetComponent<vision>().creatures[i].transform.position);
				closest = vision.GetComponent<vision>().creatures[i];
			}
		}
		
		return closest;
	}
	
	GameObject nearestAlly () {
		GameObject closest = gameObject;
		int firstCheck = 0;
		
		for (int i = 0; i < vision.GetComponent<vision>().creatures.Count; i++) {
			if (vision.GetComponent<vision>().creatures[i].GetComponent<stats>().team==gameObject.GetComponent<stats>().team) {
				closest = vision.GetComponent<vision>().creatures[i];
				firstCheck = i+1;
				break;
			}
		}
		
		float dist = Vector3.Distance (gameObject.transform.position, closest.transform.position);
		
		for (int i = firstCheck; i < vision.GetComponent<vision>().creatures.Count; i++) {
			if ((vision.GetComponent<vision>().creatures[i].GetComponent<stats>().team==gameObject.GetComponent<stats>().team)&&(Vector3.Distance(gameObject.transform.position, vision.GetComponent<vision>().creatures[i].transform.position) < dist)) {
				dist = Vector3.Distance (gameObject.transform.position, vision.GetComponent<vision>().creatures[i].transform.position);
				closest = vision.GetComponent<vision>().creatures[i];
			}
		}
		
		return closest;
	}
	
	void attack() {
		for (int i = 0; i < abilities.Length; i++) {
			if (abilities[i].isCharged())
				abilities[i].Attack();
		}
	}
	
	/*
	void attack() {
		if (GetComponent<stats>().atkchg>=GetComponent<stats>().atkspd*200) {
			if (GetComponent<stats>().range > 1) {
				float dist = Vector3.Distance (gameObject.transform.position, target.transform.position);
				if (dist <= GetComponent<stats>().range) {
					Vector3 dir = target.transform.position - gameObject.transform.position;
					dir.Normalize();
					
					GameObject missileObject = Instantiate (missile) as GameObject;
					
					missileObject.transform.position = gameObject.transform.position;
					missileObject.transform.up = dir;
					missileObject.GetComponent<Rigidbody>().AddForce (dir*2000);
					
					missileObject.GetComponent<missileController>().dmg = Random.Range(GetComponent<stats>().power-2,GetComponent<stats>().power);
					if (gameObject.GetComponent<stats>().type=="wizard")
						missileObject.GetComponent<missileController>().isPiercing = true;
					else
						missileObject.GetComponent<missileController>().isPiercing = false;
					missileObject.GetComponent<missileController>().isCold = false;
					missileObject.GetComponent<missileController>().team = gameObject.GetComponent<stats>().team;
					
					GetComponent<stats>().atkchg = 0;
				}
			} else if (inRange) {
				//Debug.Log (gameObject.name + " attacks! ");
				GetComponent<stats>().atkchg = 0;
				target.GetComponent<stats>().takeHit(Random.Range(GetComponent<stats>().power-2,GetComponent<stats>().power), false, false);	
			}
		}
		
	}*/
	
	void OnTriggerEnter (Collider other) {
		
	}
	
	void OnTriggerExit (Collider other) {
		
	}
}
