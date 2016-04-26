using UnityEngine;
using System.Collections;
using System;

public class Tile {

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


	public Immovable Immovable { get; protected set; }
	LooseObject looseObject;

	public World world { get; protected set; }

	public Job jobPending;

	public int X {get; protected set;}
	public int Y {get; protected set;}

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
			Immovable = null;
			return true;
		}

		// obj instance is null
		if (Immovable != null) {
			return false;
			//Debug.LogError("Trying to install and objInstance into an already taken tile");
		}

		// everything is fine here
		Immovable = objInstance;
		return true;
	}

	public bool IsNeighbor (Tile tile, bool checkDiagonal = false) {
		// checking if a tile in neighboring this one
		return Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1;

		if (checkDiagonal == true) {
			return (Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Y - tile.Y) == 1);
		}

		return false;
	}

}



