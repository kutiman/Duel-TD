using UnityEngine;
using System.Collections;
using System;

public class InstalledObject {

	public Tile tile {get; protected set;}

	public string objectType {get; protected set;}

	// multiplier for traversing through this obejct. Can be multiplied by the tile movement cost as well
	// for example, a movement cost of 2 will take twice the time to traverse
	// IMPORTANT: if equal to zero, objects is impassable (e.g. wall)
	float movementCost = 1f;

	// size of the object, if does not take only one tile
	int width = 1;
	int height = 1;

	Action <InstalledObject> cbOnChanged;

	static public InstalledObject CreatePrototype (string objectType, float movementCost = 1f, int width = 1, int height = 1	) {
		InstalledObject obj = new InstalledObject();

		obj.objectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;

		return obj;
	}

	static public InstalledObject PlaceInstance (InstalledObject proto, Tile tile) {
		InstalledObject obj = new InstalledObject ();
		//Debug.Log("PlaceInstance REACHED sUCCESSFULLY");
		obj.objectType = proto.objectType;
		obj.movementCost = proto.movementCost;
		obj.width = proto.width;
		obj.height = proto.height;

		obj.tile = tile;

		// placing the objcet in the tile
		if (tile.PlaceObject (obj) == false) {
			return null;
		}
		//Debug.Log(obj.objectType);
		return obj;
	}

	public void RegisterOnChangedCallback (Action<InstalledObject> callbackfunk) {
		cbOnChanged += callbackfunk;
	}

	public void UnregisterOnChangedCallback (Action<InstalledObject> callbackfunk) {
		cbOnChanged -= callbackfunk;
	}
}
