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


	InstalledObject installedObject;
	LooseObject looseObject;

	World world;

	public int X {get; protected set;}
	public int Y {get; protected set;}

	public Tile (World world, int x, int y) {
		this.world = world;
		this.X = x;
		this.Y = y;
	}

	public void RegisterTileTypeChangedCallback (Action<Tile> callback) {
		cbTileTypeChanged += callback;
	}

	public void UnregisterTileTypeChangedCallback (Action<Tile> callback) {
		cbTileTypeChanged -= callback;
	}
}



