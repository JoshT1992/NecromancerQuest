/*
** Loads all xml documents and stores them in one object that all other objects
** can then access. This is done so every instance of code isn't loading
** excess files.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class loadXml : MonoBehaviour {
	public XmlDocument xmlDoc;

	void Awake() {
		xmlDoc = new XmlDocument();
		xmlDoc.Load ("Assets\\Resources\\spellBook.xml");
	}

}