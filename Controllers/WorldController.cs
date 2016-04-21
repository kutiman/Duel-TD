using UnityEngine;
using System.Collections.Generic;
using System;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }
	
	public World World {get; protected set;}

	Dictionary <Tile, GameObject> tileGameObjectMap;
	Dictionary <InstalledObject, GameObject> installedObjectGameObjectMap;
	Dictionary <string, GameObject> itemsMap;

	public GameObject[] itemsList;

	public Sprite groundSprite;

	void Start () {

		if (Instance != null) Debug.Log("Too many World controllers");
		Instance = this;
		
		// create a new wrd with tiles
		World = new World ();

		World.RegisterInstalledObjectCreated (OnInstalledObjectCreated);

		tileGameObjectMap = new Dictionary <Tile, GameObject>();
		installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();
		itemsMap = PopulateItemsGameObjectsDictionary(itemsList);

		// Creating a gameobject for each of the tiles
		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {

				Tile tile_data = World.GetTileAt (x, y);

				GameObject tile_go = new GameObject ();

				tileGameObjectMap.Add(tile_data, tile_go);

				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3(tile_data.X, 0.01f, tile_data.Y);
				tile_go.transform.SetParent(this.transform, true);
				tile_go.transform.Rotate(Vector3.left * -90);

				SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer> ();

				// register a tile changing type callback
				tile_data.RegisterTileTypeChangedCallback( OnTileTypeChanged );


			}
		}
	}
	
	void Update () {

	}

	void OnTileTypeChanged (Tile tile_data) {

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
			Debug.LogError("OnTileTypeChanged - Unrecognized tile type");
		}
	}

	public Tile GetTileAtWorldCoord (Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.z);

		return World.GetTileAt(x, y);
	}

	public void OnInstalledObjectCreated (InstalledObject obj) {

		GameObject obj_go = Instantiate(itemsMap[obj.objectType]);

		installedObjectGameObjectMap.Add(obj, obj_go);

		obj_go.name = obj.objectType + "_"+ obj.tile.X + "_" + obj.tile.Y;
		obj_go.transform.position = new Vector3(obj.tile.X, 0, obj.tile.Y);
		obj_go.transform.SetParent(this.transform, true);


		// register a tile changing type callback
		obj.RegisterOnChangedCallback( OnInstalledObjectChanged );
	}

	void OnInstalledObjectChanged (InstalledObject obj) {
		Debug.LogError ("Not Implemented!");
	}

	Dictionary <string, GameObject> PopulateItemsGameObjectsDictionary (GameObject[] goArray) {
		Dictionary <string, GameObject> dict = new Dictionary <string, GameObject> ();
		foreach (GameObject obj in goArray) {
			dict.Add (obj.name, obj);
			Debug.Log(obj.name);
		}
		return dict;
	}
}
