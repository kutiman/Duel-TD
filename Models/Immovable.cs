using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Immovable : IXmlSerializable {

	public Dictionary<string, float> imvbParameters;
	public Action<Immovable, float> updateActions;

	public void Update(float deltaTime) {
		if(updateActions != null) {
			updateActions(this, deltaTime);
		}
	}

	public Tile tile {get; protected set;}

	public string objectType {get; protected set;}

	// multiplier for traversing through this obejct. Can be multiplied by the tile movement cost as well
	// for example, a movement cost of 2 will take twice the time to traverse
	// IMPORTANT: if equal to zero, objects is impassable (e.g. wall)
	public float movementCost {get; protected set;}

	// size of the object, if does not take only one tile
	int width;
	int height;
	int depth;

	Action <Immovable> cbOnChanged;

	// empty constructor for xml serialization
	public Immovable () {
		imvbParameters = new Dictionary<string, float>();
	}

	/// Copy constructor
	protected Immovable (Immovable other) {

		this.objectType = other.objectType;
		this.movementCost = other.movementCost;
		this.width = other.width;
		this.height = other.height;
		this.depth = other.depth;

		this.imvbParameters = new Dictionary<string, float> (other.imvbParameters);

		if (other.updateActions != null) {
			this.updateActions = (Action<Immovable, float>)other.updateActions.Clone();
		}
	}

	virtual public Immovable Clone () {
		return new Immovable(this);
	}

	public Immovable (string objectType, float movementCost = 1f, int width = 1, int height = 1, int depth = 1	) {
		this.objectType = objectType;
		this.movementCost = movementCost;
		this.width = width;
		this.height = height;
		this.depth = depth;

		imvbParameters = new Dictionary<string, float>();
	}

	static public Immovable PlaceInstance (Immovable proto, Tile tile) {

		Immovable obj = proto.Clone();

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
		writer.WriteAttributeString("Y", tile.Y.ToString());
		writer.WriteAttributeString("Z", tile.Z.ToString());
		writer.WriteAttributeString("ObjectType", objectType);

		foreach (string k in imvbParameters.Keys) {
			writer.WriteStartElement("Param");
			writer.WriteAttributeString("name", k);
			writer.WriteAttributeString("value", imvbParameters[k].ToString());
			writer.WriteEndElement();
		}
	}

	public void ReadXml (XmlReader reader) {
		if (reader.ReadToDescendant("Param")) {

			do {
				string k = reader.GetAttribute("name"); //int.Parse (reader.GetAttribute ("X"));
				float v = float.Parse (reader.GetAttribute("value"));
				imvbParameters[k] = v;
			} while(reader.ReadToNextSibling("Param"));


		}
	}
}
