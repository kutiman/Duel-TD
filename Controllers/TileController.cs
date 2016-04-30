using UnityEngine;
using System.Collections.Generic;
using System;

public class TileController : MonoBehaviour {


	World world { get { return WorldController.Instance.world; } }

	Dictionary <Tile, GameObject> tileGameObjectMap;

	public Sprite groundSprite;

	void Start () {

		tileGameObjectMap = new Dictionary <Tile, GameObject>();

		// Creating a gameobject for each of the tiles
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				for (int z = 0; z < world.Depth; z++) {
					Tile tile_data = world.GetTileAt (x, y, z);

					GameObject tile_go = new GameObject ();

					tileGameObjectMap.Add(tile_data, tile_go);

					tile_go.name = "Tile_" + x + "_" + y + "_" + z;
					tile_go.transform.position = new Vector3(tile_data.X, tile_data.Z + 0.01f, tile_data.Y);
					tile_go.transform.SetParent(this.transform, true);
					tile_go.transform.Rotate(Vector3.left * -90);

					tile_go.AddComponent<SpriteRenderer> ();

					OnTileChanged(tile_data);
				}

					
			}
		}

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
}