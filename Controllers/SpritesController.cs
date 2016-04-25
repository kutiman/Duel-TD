using UnityEngine;
using System.Collections.Generic;
using System;

public class SpritesController : MonoBehaviour {


	public World world { get { return WorldController.Instance.world; } }

	Dictionary <Tile, GameObject> tileGameObjectMap;
	Dictionary <Immovable, GameObject> ImmovableGameObjectMap;
	Dictionary <string, GameObject> itemsMap;

	// this list is populated in the inspector. Takes all the items for installing in the game;
	public GameObject[] itemsList;

	public Sprite groundSprite;

	void Start () {

		tileGameObjectMap = new Dictionary <Tile, GameObject>();
		ImmovableGameObjectMap = new Dictionary<Immovable, GameObject>();
		itemsMap = PopulateItemsGameObjectsDictionary(itemsList);

		world.RegisterImmovableCreated (OnImmovableCreated);
		world.RegisterTileChanged (OnTileChanged);

	} 
	void OnTileChanged (Tile tile_data) {

		if (tileGameObjectMap.ContainsKey (tile_data) == false) {
			Debug.LogError ("tile_data does not exist in the dictionary. You need to assign one");
			return;
		}

		GameObject tile_go = tileGameObjectMap [tile_data];
		if (tile_go == null) {
			Debug.LogError ("tile_go does not exist");
			return;
		}

		if (tile_data.Type == Tile.TileType.Ground) {
			tile_go.GetComponent<SpriteRenderer>().sprite = groundSprite;
		}
		else if (tile_data.Type == Tile.TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer>().sprite = null;
		}
		else {
			Debug.LogError("OnTileChanged - Unrecognized tile type");
		}
	}

		public void OnImmovableCreated (Immovable obj) {

		GameObject obj_go = Instantiate(itemsMap[obj.objectType]);

		ImmovableGameObjectMap.Add(obj, obj_go);

		obj_go.name = obj.objectType + "_"+ obj.tile.X + "_" + obj.tile.Y;
		obj_go.transform.position = new Vector3(obj.tile.X, 0, obj.tile.Y);
		obj_go.transform.SetParent(this.transform, true);

		// Debug

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
