using UnityEngine;
using System.Collections.Generic;
using System;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }
	
	public World world {get; protected set;}

	void OnEnable () {

		if (Instance != null) Debug.Log("Too many world controllers");

		Instance = this;

		// create a new world with tiles
		world = new World ();
	}
	

	public Tile GetTileAtWorldCoord (Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.z);

		return world.GetTileAt(x, y);
	}
}
