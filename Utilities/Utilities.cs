using UnityEngine;
using System.Collections.Generic;

public class Utilities {

	public static Dictionary <string, GameObject> PopulateGameObjectsDictionary (GameObject[] goArray) {
		Dictionary <string, GameObject> dict = new Dictionary <string, GameObject> ();
		foreach (GameObject obj in goArray) {
			dict.Add (obj.name, obj);
		}
		return dict;
	}
}

