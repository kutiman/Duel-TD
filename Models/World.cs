using UnityEngine;
using System.Collections.Generic;
using System;

public class World {

	Tile[,] tiles;

	public int Width {get; protected set;}
	public int Height {get; protected set;}

	Action<InstalledObject> cbInstalledObjectCreated;

	Dictionary<string, InstalledObject> installedObjectsPrototypes; 

	public World (int width = 30, int height = 30) {
		this.Width = width;
		this.Height = height;

		tiles = new Tile[width, height];

		// creating the tiles
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x, y] = new Tile(this, x, y);
			}
		}

		// Creating the map of installed objects
		installedObjectsPrototypes = new Dictionary<string, InstalledObject>();

		InstalledObject wallPrototype = InstalledObject.CreatePrototype (
			"Wall",
			0, // impassable
			1, // width
			1 //height
		);
		installedObjectsPrototypes.Add("Wall", wallPrototype);

		InstalledObject barrelPrototype = InstalledObject.CreatePrototype (
			"Barrel",
			0, // impassable
			1, // width
			1 //height
		);
		installedObjectsPrototypes.Add("Barrel", barrelPrototype);

		InstalledObject Tree_Pine_01Prototype = InstalledObject.CreatePrototype (
			"Tree_Pine_01",
			0.5f, // impassable
			1, // width
			1 //height
		);
		installedObjectsPrototypes.Add("Tree_Pine_01", Tree_Pine_01Prototype);
	}

	public Tile GetTileAt (int x, int y) {
		if (x < 0 || x >= Width || y < 0 || y >= Height) {
			//Debug.LogError("Tile ("+x+","+y+") is out of range");
			return null;
		}
		return tiles[x, y];
	}

	public void PlaceInstalledObject (string objectType, Tile t) {
		if (installedObjectsPrototypes.ContainsKey (objectType) == false) {
			Debug.LogError ("installedObjectsPrototypes doesnt contain an prototype for key " + objectType);
			return;
		}

		InstalledObject obj = InstalledObject.PlaceInstance (installedObjectsPrototypes [objectType], t);

		if (obj == null) {
			// Failed to place object -- most likely there was already something there.
			return;
		}

		if(cbInstalledObjectCreated != null) {
			cbInstalledObjectCreated(obj);
		}
	}

	public void RegisterInstalledObjectCreated (Action<InstalledObject> callbackfunc) {
		cbInstalledObjectCreated += callbackfunc;
	}

	public void UnregisterInstalledObjectCreated (Action<InstalledObject> callbackfunc) {
		cbInstalledObjectCreated -= callbackfunc;
	}

}
