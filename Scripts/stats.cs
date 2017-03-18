using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stats : MonoBehaviour {
	public int hp, maxhp, power, armour, speed, range, team;
	public string type;
	
	public GameObject healthCube;
	
	public List<GameObject> cubes;
	
	private float radius;
	
	private int remCubes;
	
	private int numCubes;
	
	void Start () {
		if (type=="vivianna") {
			maxhp = 30;
			power = 1;
			armour = 0;
			speed = 12;
			range = 1;
			team = 1;
		}
		if (type=="zombie") {
			maxhp = 10;
			power = 4;
			armour = 0;
			speed = 4;
			range = 1;
			team = 1;
		}
		if (type=="guard") {
			maxhp = 8;
			power = 4;
			armour = 1;
			speed = 6;
			range = 1;
			team = 2;
		}
		if (type=="archer") {
			maxhp = 6;
			power = 2;
			armour = 0;
			speed = 6;
			range = 12;
			team = 2;
		}
		if (type=="wizard") {
			maxhp = 6;
			power = 3;
			armour = 0;
			speed = 3;
			range = 12;
			team = 2;
		}
		
		hp = maxhp;
		if (maxhp <= 27) {
			radius = 0.2f;
		} else if (maxhp > 27) {
			radius = 0.4f;
		}
		createHealth(hp);
	}
	
	void Update () {
		if (remCubes > 0) {
			if (numCubes > 0) {
				cubes[numCubes-1].AddComponent<Rigidbody>();
				cubes[numCubes-1].transform.parent = null;
				float x = Random.Range(-400,400);
				float y = 200;
				Vector3 rForce = new Vector3 (x,y,x);
				cubes[numCubes-1].GetComponent<Rigidbody>().AddForce (rForce);
				cubes[numCubes-1].GetComponent<cubeDeletor>().broken = true;
			}
			remCubes--;
			numCubes--;
		}
	}
	
	//When a unit takes damage from any source, a takeHit is called
	//First it asks if the damage is cold damage, which does not hurt the player and only slows them a % amount based on current hp
	//Then it checks if the unit has more armour than the damage being dealt. If so it does 0 damage. This prevents negative damage from healing units.
	//Next it checks if the attack has armour piercing. If it does, it ignores armour.
	//Finally if none of the above conditions are met, damage will be applied normally after having the unit's armour value subtracted from it.
	public void takeHit (int dmg, bool isPiercing, bool isCold) {
		if (isCold) {
			float slowAmount = dmg/hp;
			speed = (int)(speed*(1-slowAmount));
		} else if (isPiercing) {
			hp -= dmg;
			remCubes += dmg;
		} else if (dmg-armour<=0) {
			//Report no damage
		} else {
			hp -= dmg-armour;
			remCubes += dmg-armour;
		}
	}
	
	public void createHealth (int health) {
		int num = 0;
		int col = 0;
		Vector3 center = new Vector3 (gameObject.transform.position.x,gameObject.transform.position.y+2,gameObject.transform.position.z);
		for (float i = -radius; i <= radius; i+=0.2f) {
			for (float j = -radius; j <= radius; j+=0.2f) {
				for (float k = -radius; k <= radius; k+=0.2f) {
					Vector3 pos = new Vector3(j,i,k);
					pos += center;
					
					GameObject newCube = Instantiate (healthCube) as GameObject;
					newCube.transform.position = pos;
					newCube.transform.parent = gameObject.transform;
					Color c = new Color (0,0,0,0);
					if (col==0)
						c = new Color (0,0.8f,0.2f,1);
					if (col==1)
						c = new Color (0,0.8f,0.4f,1);
					if (col==2)
						c = new Color (0,0.6f,0.6f,1);
					if (col==3)
						c = new Color (0.4f,0.4f,0.8f,1);
					if (col>=4)
						c = new Color (0.8f,0,0.4f,1);
						
						
					newCube.GetComponent<Renderer>().material.color = c;//new Color(Mathf.Abs(i-radius)*1.5f,(1-Mathf.Abs(i-radius))*1.5f,0,1);
					cubes.Add(newCube);
					num++;
					
					if (num >= health)
						break;
				}
				if (num >= health)
					break;
			}
			if (num >= health)
				break;
			col++;
		}
		numCubes = cubes.Count;
	}
	
	public bool isDead () {
		if (hp <= 0)
			return true;
		return false;
	}
}
