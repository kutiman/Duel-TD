using UnityEngine;
using System.Collections.Generic;
using System;

public class ImmovablesController : MonoBehaviour {

	public World world { get { return WorldController.Instance.world; } }

	Dictionary <Immovable, GameObject> immovableGameObjectMap;
	public Dictionary <string, GameObject> itemsMap { get; protected set; }

	// this list is populated in the inspector. Takes all the items for installing in the game;
	public GameObject[] itemsList;

	void Start () {

		immovableGameObjectMap = new Dictionary<Immovable, GameObject>();
		itemsMap = Utilities.PopulateGameObjectsDictionary(itemsList);

		world.RegisterImmovableCreated (OnImmovableCreated);

		// go through existing immovables, as from save file...
		foreach(Immovable imvb in world.immovables) {
			OnImmovableCreated(imvb);
		}
	} 

	public void OnImmovableCreated (Immovable obj) {

		GameObject obj_go = Instantiate(itemsMap[obj.objectType]);

		immovableGameObjectMap.Add(obj, obj_go);

		obj_go.name = obj.objectType + "_"+ obj.tile.X + "_" + obj.tile.Y;
		obj_go.transform.position = new Vector3(obj.tile.X, 0, obj.tile.Y);
		obj_go.transform.SetParent(this.transform, true);

		// register a tile changing type callback
		obj.RegisterOnChangedCallback( OnImmovableChanged );
	}

	void OnImmovableChanged (Immovable obj) {
		Debug.LogError ("Not Implemented!");
	}
}
