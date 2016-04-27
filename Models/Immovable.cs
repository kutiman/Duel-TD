using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Immovable : IXmlSerializable {

	public Tile tile {get; protected set;}

	public string objectType {get; protected set;}

	// multiplier for traversing through this obejct. Can be multiplied by the tile movement cost as well
	// for example, a movement cost of 2 will take twice the time to traverse
	// IMPORTANT: if equal to zero, objects is impassable (e.g. wall)
	public float movementCost {get; protected set;}

	// size of the object, if does not take only one tile
	int width = 1;
	int height = 1;

	Action <Immovable> cbOnChanged;

	static public Immovable CreatePrototype (string objectType, float movementCost = 1f, int width = 1, int height = 1	) {
		Immovable obj = new Immovable();

		obj.objectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;

		return obj;
	}

	static public Immovable PlaceInstance (Immovable proto, Tile tile) {
		Immovable obj = new Immovable ();
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

	public void RegisterOnChangedCallback (Action<Immovable> callbackfunk) {
		cbOnChanged += callbackfunk;
	}

	public void UnregisterOnChangedCallback (Action<Immovable> callbackfunk) {
		cbOnChanged -= callbackfunk;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///
	///												SAVE & LOAD
	///
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public XmlSchema GetSchema () {
		return null;
	}

	public void WriteXml (XmlWriter writer) {
		writer.WriteAttributeString("X", tile.X.ToString());
		writer.WriteAttributeString("X", tile.Y.ToString());
		writer.WriteAttributeString("ObjectType", objectType);
	}

	public void ReadXml (XmlReader reader) {

	}
}
