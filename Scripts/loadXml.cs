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