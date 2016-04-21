using UnityEngine;
using System.Collections;
using System;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }
	
	public World World {get; protected set;}

	public Sprite groundSprite;

	void Start () {

		if (Instance != null) Debug.Log("Too many World controllers");
		Instance = this;
		
		// create a new wrd with tiles
		World = new World ();

		// Creating a gameobject for each of the tiles
		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				GameObject tile_go = new GameObject ();
				tile_go.name = "Tile_" + x + "_" + y;

				Tile tile_data = World.GetTileAt (x, y);
				tile_go.transform.position = new Vector3(tile_data.X, 0.01f, tile_data.Y);
				tile_go.transform.SetParent(this.transform, true);
				tile_go.transform.Rotate(Vector3.left * -90);

				SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer> ();

				tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); } );
			}
		}
	}
	
	void Update () {
	
	}

	void OnTileTypeChanged (Tile tile_data, GameObject tile_go) {
		tile_go.GetComponent<SpriteRenderer>().sprite = tile_data.Type == Tile.TileType.Ground ? groundSprite : null;
	}

	public Tile GetTileAtWorldCoord (Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.z);

		return World.GetTileAt(x, y);
	}
}
