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

		// setting the camera to the middle of the world
		Camera.main.transform.position = new Vector3 (world.Width/2, Camera.main.transform.position.y, world.Height/2 + Camera.main.transform.position.z);
	}

	public void Update () {
		world.Update(Time.deltaTime);
	}
	

	public Tile GetTileAtWorldCoord (Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.z);

		return world.GetTileAt(x, y);
	}
}
