using UnityEngine;
using System.Collections.Generic;
using System;

public class ImmovablesController : MonoBehaviour {

	public World world { get { return WorldController.Instance.world; } }

	Dictionary <Immovable, GameObject> ImmovableGameObjectMap;
	public Dictionary <string, GameObject> itemsMap { get; protected set; }

	// this list is populated in the inspector. Takes all the items for installing in the game;
	public GameObject[] itemsList;

	void Start () {

		ImmovableGameObjectMap = new Dictionary<Immovable, GameObject>();
		itemsMap = PopulateItemsGameObjectsDictionary(itemsList);

		world.RegisterImmovableCreated (OnImmovableCreated);
	} 

	public void OnImmovableCreated (Immovable obj) {

		GameObject obj_go = Instantiate(itemsMap[obj.objectType]);

		ImmovableGameObjectMap.Add(obj, obj_go);

		obj_go.name = obj.objectType + "_"+ obj.tile.X + "_" + obj.tile.Y;
		obj_go.transform.position = new Vector3(obj.tile.X, 0, obj.tile.Y);
		obj_go.transform.SetParent(this.transform, true);

		// register a tile changing type callback
		obj.RegisterOnChangedCallback( OnImmovableChanged );
	}

	void OnImmovableChanged (Immovable obj) {
		Debug.LogError ("Not Implemented!");
	}

	Dictionary <string, GameObject> PopulateItemsGameObjectsDictionary (GameObject[] goArray) {
		Dictionary <string, GameObject> dict = new Dictionary <string, GameObject> ();
		foreach (GameObject obj in goArray) {
			dict.Add (obj.name, obj);
		}
		return dict;
	}
}
