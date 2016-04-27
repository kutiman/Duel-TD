using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Tile : IXmlSerializable {

	public enum TileType { Empty, Ground };

	TileType type = TileType.Empty;

	Action<Tile> cbTileTypeChanged;

	public TileType Type {
		get {
			return type;
		}
		set {
			TileType oldType = type;
			type = value;
			if (cbTileTypeChanged != null && oldType != type) {
				cbTileTypeChanged(this);

			}
		}
	}


	public Immovable immovable { get; protected set; }
	LooseObject looseObject;

	public World world { get; protected set; }

	public Job jobPending;

	public int X {get; protected set;}
	public int Y {get; protected set;}

	public float movementCost {
		get { 
			if (immovable == null) {
				return 1;
			}
			else {
				return 1 * immovable.movementCost;
			}
		}
	}

	public Tile (World world, int x, int y) {
		this.world = world;
		this.X = x;
		this.Y = y;
	}

	public void RegisterTileChangedCallback (Action<Tile> callback) {
		cbTileTypeChanged += callback;
	}

	public void UnregisterTileChangedCallback (Action<Tile> callback) {
		cbTileTypeChanged -= callback;
	}

	public bool PlaceObject (Immovable objInstance) {
		if (objInstance == null) {
			// we are uninstalling the object that currently occupies the tile
			immovable = null;
			return true;
		}

		// obj instance is null
		if (immovable != null) {
			return false;
			//Debug.LogError("Trying to install and objInstance into an already taken tile");
		}

		// everything is fine here
		immovable = objInstance;
		return true;
	}

	public bool IsNeighbor (Tile tile, bool checkDiagonal = false) {
		// checking if a tile in neighboring this one
		return ( 
			(Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1) || 
			(checkDiagonal == true && Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Y - tile.Y) == 1)
		);
	}

	public Tile[] GetNeighbors (bool checkDiagonal = false) {
		Tile[] ns;
		// nighboring tiles could be null

		if (checkDiagonal == false) {
			ns = new Tile[4];
		}
		else {
			ns = new Tile[8];
		}

		Tile n;

		// order of neighbors is N E S W NE SE SW NW
		n = world.GetTileAt (X, Y + 1);
		ns [0] = n;
		n = world.GetTileAt (X + 1, Y);
		ns [1] = n;
		n = world.GetTileAt (X, Y - 1);
		ns [2] = n;
		n = world.GetTileAt (X - 1, Y);
		ns [3] = n;

		if (checkDiagonal == true) {
			n = world.GetTileAt (X + 1, Y + 1);
			ns [4] = n;
			n = world.GetTileAt (X + 1, Y - 1);
			ns [5] = n;
			n = world.GetTileAt (X - 1, Y - 1);
			ns [6] = n;
			n = world.GetTileAt (X - 1, Y + 1);
			ns [7] = n;
		}

		return ns;
	}


	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///
	///												SAVE & LOAD
	///
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// empty constructor for xml serialization
//	public Tile () {
//		
//	}

	public XmlSchema GetSchema () {
		return null;
	}

	public void WriteXml (XmlWriter writer) {
		writer.WriteAttributeString("X", X.ToString());
		writer.WriteAttributeString("X", Y.ToString());
		writer.WriteAttributeString("Type", ((int) Type).ToString());
	}

	public void ReadXml (XmlReader reader) {

		type = (Tile.TileType) int.Parse(reader.GetAttribute("Type"));
	}


}



