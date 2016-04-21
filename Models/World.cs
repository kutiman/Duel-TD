using UnityEngine;
using System.Collections;

public class World {

	Tile[,] tiles;

	public int Width {get; protected set;}
	public int Height {get; protected set;}

	public World (int width = 30, int height = 30) {
		this.Width = width;
		this.Height = height;

		tiles = new Tile[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x, y] = new Tile(this, x, y);
			}
		}
	}

	public Tile GetTileAt (int x, int y) {
		if (x < 0 || x >= Width || y < 0 || y >= Height) {
			//Debug.LogError("Tile ("+x+","+y+") is out of range");
			return null;
		}
		return tiles[x, y];
	}

}
