using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.IO;

public class abilityController : MonoBehaviour {
	public int power, chargeMax, charge, attacks, cost;
	public bool isMagic, isRanged, isArea, isCold, isPiercing;
	public bool[] effects;
	public string name;
	
	public GameObject missile;
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
		
		foreach (XmlNode node in xml.DocumentElement.ChildNodes) {
			if (node.Attributes["name"].InnerText==name) {
				power = int.Parse(node.Attributes["power"].InnerText);
				attacks = int.Parse(node.Attributes["attacks"].InnerText);
				cost = int.Parse(node.Attributes["cost"].InnerText);
				chargeMax = int.Parse(node.Attributes["charge"].InnerText);
				isArea = bool.Parse(node.Attributes["isArea"].InnerText);
				isRanged = bool.Parse(node.Attributes["isRanged"].InnerText);
				isPiercing = bool.Parse(node.Attributes["isPiercing"].InnerText);
				isCold = bool.Parse(node.Attributes["isCold"].InnerText);
				break;
			}
		}
	}
	
	void Update () {
		if (charge < chargeMax)
			charge++;
	}
	
	void Attack () {
		GameObject target;
		if ((target=gameObject.GetComponent<AIController>().target)==null)
			 target = gameObject.GetComponent<PlayerController>().target;
		if (isArea) { //AoE Attack
			//Make the AoE object at the target location
			GameObject areaObject = Instantiate (area) as GameObject;
			if (isRanged)
				areaObject.transform.position = target.transform.position;
			else
				areaObject.transform.position = gameObject.transform.position;
			//Scan for each object in the AoE
			List<GameObject> objects = areaObject.GetComponent<vision>().creatures;
			//Apply power to all of them
			for (int i = 0; i < objects.Count; i++) {
				if (objects[i].GetComponent<stats>().team != gameObject.GetComponent<stats>().team)
					objects[i].GetComponent<stats>().takeHit(Random.Range(power-1,power+1), effects[0], effects[1]);
			}
			//Destroy the Area
			Destroy (areaObject);
		} else if (isRanged) { //Ranged Attack
			float dist = Vector3.Distance (gameObject.transform.position, target.transform.position);
				if (dist <= GetComponent<stats>().range) {
					Vector3 dir = target.transform.position - gameObject.transform.position;
					dir.Normalize();
					
					GameObject missileObject = Instantiate (missile) as GameObject;
					
					missileObject.transform.position = gameObject.transform.position;
					missileObject.transform.up = dir;
					missileObject.GetComponent<Rigidbody>().AddForce (dir*2000);
					
					missileObject.GetComponent<missileController>().dmg = Random.Range(power-1,power+1);
					missileObject.GetComponent<missileController>().isPiercing = effects[0];
					missileObject.GetComponent<missileController>().isCold = effects[1];
					missileObject.GetComponent<missileController>().team = gameObject.GetComponent<stats>().team;
				}
		} else if (gameObject.GetComponent<AIController>().inRange) { //Melee attack
			target.GetComponent<stats>().takeHit(Random.Range(power-1,power+1), effects[0], effects[1]);
		}
		
		charge = 0;
	}
	
	public bool isCharged () {
		return (charge>=chargeMax);
	}
}
