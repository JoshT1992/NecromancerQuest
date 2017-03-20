using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.IO;

public class abilityController : MonoBehaviour {
	public int power, chargeMax, charge, attacks, cost;
	public bool isMagic, isRanged, isArea, isCold, isPiercing;
	public bool[] effects;
	public string abilityName;
	
	public GameObject missile;
	public GameObject melee;
	public GameObject area;
	
	public GameObject loadXml;
	
	//Effects are held in a list since multiple effects can be on one attack
	//
	//Effect 0: Piercing
	//The attack ignores armour
	//
	//Effect 1: Freezing
	//Attack does not deal power, but instead applies a freezing effect
	//The effect is based on a calculation of power / enemyCurrentHp
	//The effect stacks, but decays over time. The decay period starts
	//after a second or so.
	//When the slow reaches a certain threshold it freezes the enemy

	void Start () {
		charge = 0;
		effects = new bool[10];
		for (int i = 0; i < 10; i++)
			effects[i] = false;
		
		XmlDocument xml = loadXml.GetComponent<loadXml>().xmlDoc;
		
		bool abilityFound = false;
		
		foreach (XmlNode node in xml.DocumentElement.ChildNodes) {
			if (node.Attributes["name"].InnerText==abilityName) {
				power = int.Parse(node.Attributes["power"].InnerText);
				attacks = int.Parse(node.Attributes["attacks"].InnerText);
				cost = int.Parse(node.Attributes["cost"].InnerText);
				chargeMax = int.Parse(node.Attributes["charge"].InnerText) * 100;
				isArea = bool.Parse(node.Attributes["isArea"].InnerText);
				isRanged = bool.Parse(node.Attributes["isRanged"].InnerText);
				isPiercing = bool.Parse(node.Attributes["isPiercing"].InnerText);
				isCold = bool.Parse(node.Attributes["isCold"].InnerText);
				abilityFound = true;
				break;
			}
		}
		
		if (!abilityFound) {
			power = 0;
			attacks = 0;
			cost = 0;
			chargeMax = -1;
			isArea = false;
			isRanged = false;
			isPiercing = false;
			isCold = false;
		}
	}
	
	void Update () {
		if (charge < chargeMax)
			charge++;
	}
	
	public void Attack () {
		charge = 0;
		StartCoroutine("launchAttack");
	}
	
	IEnumerator launchAttack () {
		GameObject target;
		if ((target=gameObject.GetComponent<AIController>().target)==null)
			 target = gameObject.GetComponent<PlayerController>().target;
		
		for (int k = 0; k < attacks; k++) {
			if (target!=null) {
				if (isArea) { //AoE Attack
					//Make the AoE object at the target location
					Vector3 centerPoint = new Vector3(0,0,0);
					if (isRanged)
						centerPoint = target.transform.position;
					else
						centerPoint = gameObject.transform.position;
					//Make the boom
					StartCoroutine ("testArea", centerPoint);
					//Get the list of objects the unit can see
					List<GameObject> objects = gameObject.GetComponent<AIController>().vision.GetComponent<vision>().creatures;
					//Apply power to all of them if they are enemies and within 2 meters
					for (int i = 0; i < objects.Count; i++) {
						if ((objects[i].GetComponent<stats>().team != gameObject.GetComponent<stats>().team)&&(Vector3.Distance(centerPoint, objects[i].transform.position) <= 2))
							//objects[i].GetComponent<stats>().takeHit(Random.Range(power-1,power+1), effects[0], effects[1]);
							objects[i].GetComponent<stats>().takeHit(power, effects[0], effects[1]);
					}
				} else if (isRanged) { //Ranged Attack
					float dist = Vector3.Distance (gameObject.transform.position, target.transform.position);
						if (dist <= GetComponent<stats>().range) {
							Vector3 dir = target.transform.position - gameObject.transform.position;
							dir.Normalize();
							
							GameObject missileObject = Instantiate (missile) as GameObject;
							
							missileObject.transform.position = new Vector3 (gameObject.transform.position.x,gameObject.transform.position.y+1,gameObject.transform.position.z);
							missileObject.transform.up = dir;
							missileObject.GetComponent<Rigidbody>().AddForce (dir*1500);
							
							//missileObject.GetComponent<missileController>().dmg = Random.Range(power-1,power+1);
							missileObject.GetComponent<missileController>().dmg = power;
							missileObject.GetComponent<missileController>().isPiercing = effects[0];
							missileObject.GetComponent<missileController>().isCold = effects[1];
							missileObject.GetComponent<missileController>().team = gameObject.GetComponent<stats>().team;
						}
				} else if (gameObject.GetComponent<AIController>().inRange) { //Melee attack
					//target.GetComponent<stats>().takeHit(Random.Range(power-1,power+1), effects[0], effects[1]);
					target.GetComponent<stats>().takeHit(power, effects[0], effects[1]);
					StartCoroutine ("testAttack", target);
				}
				
				yield return new WaitForSeconds(0.3f);
			}
		}
	}
	
	IEnumerator testAttack (GameObject target) {
		GameObject slashObject = Instantiate (melee) as GameObject;
		
		slashObject.transform.position = gameObject.transform.position;
		slashObject.transform.parent = gameObject.transform;
		slashObject.transform.up = gameObject.transform.right;
		
		for (int i = 0; i < 100; i++) {
			slashObject.transform.Rotate (gameObject.transform.right, 500*Time.deltaTime);
			yield return null;
		}
		
		Destroy (slashObject);
	}
	
	IEnumerator testArea (Vector3 target) {
		GameObject areaObject = Instantiate (area) as GameObject;
		
		areaObject.transform.position = target;
		for (int i = 0; i < 100; i++) {
			areaObject.transform.localScale *= 1.01f;
			yield return null;
		}
		
		Destroy (areaObject);
	}
	
	public bool isCharged () {
		if (chargeMax >= 0)
			return (charge>=chargeMax);
		return false;
	}
}
